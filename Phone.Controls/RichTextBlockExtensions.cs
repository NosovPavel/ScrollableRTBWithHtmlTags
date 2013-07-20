using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace WinRTExtensions
{
    public class RichTextBlockExtensions
    {
        public static readonly DependencyProperty HtmlContentProperty = DependencyProperty.RegisterAttached("HtmlContent", typeof(string), typeof(RichTextBlockExtensions), new PropertyMetadata(default(string), new PropertyChangedCallback(HtmlContentChanged)));

        private static void HtmlContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextBlock = d as RichTextBox;
            if (richTextBlock != null)
            {
                var paragrapth = new HtmlToParagraphConvertor().GetParagraps(e.NewValue as string);
                richTextBlock.Blocks.Clear();
                richTextBlock.Blocks.Add(paragrapth);
            }
        }

        public static string GetHtmlContent(RichTextBox element)
        {
            return (string)element.GetValue(HtmlContentProperty);
        }

        public static void SetHtmlContent(RichTextBox element, string value)
        {
            element.SetValue(HtmlContentProperty, value);
        }
    }

    class ListArrayToHtml
    {
        public void GetInlines(InlineCollection result, List<string> elements)
        {

            if (!elements.Any())
            {
                return;
            }

            while (elements.Any())
            {
                var isHandled = HandleA(result, elements) ||
                                HandleText(result, elements) ||
                                HandleImage(result, elements) ||
                                HandleBR(result, elements) ||
                                HandleNOBR(result,elements) ||
                                HandleSimpleTags<Bold>(result, elements, "b") ||
                                HandleSimpleTags<Bold>(result, elements, "h1") ||
                                HandleSimpleTags<Bold>(result, elements, "strong") ||
                                HandleSimpleTags<Italic>(result, elements, "i") ||
                                HandleSimpleTags<Italic>(result, elements, "em") ||
                                HandleSimpleTags<Underline>(result, elements, "u") ||
                                HandleParagraph(result, elements);
                if (isHandled) continue;
                if (elements.Any())
                    elements.RemoveAt(0);
            }

        }

        private bool HandleA(InlineCollection result, List<string> elements)
        {
            string startTag = "<a ";
            string endTag = "</a>";
            if (!elements.Any())
            {
                return false;
            }

            var item = elements.First();
            if (item.StartsWith("<a ", StringComparison.OrdinalIgnoreCase))
            {
                var href = GetAttribute(item + "</a>", "href");
                elements.RemoveAt(0);
                if (href != null)
                {
                    var inElements = GetElements(elements, startTag, endTag);
                    var content = (String.Join(" ", inElements));

                    UIElement uiContent;
                    if (inElements.Any(IsImage))
                    {
                        uiContent = new Image()
                        {
                            Source = new BitmapImage(new Uri(GetImageSource(inElements.First(IsImage))))
                        };
                    }
                    else
                    {
                        uiContent = new TextBlock()
                        {
                            Text = content,
                            Margin = new Thickness(-6, 0, -6, -9)
                        };
                    }
                    try
                    {
                        //var hyperlinkButton = new HyperlinkButton()
                        //    {
                        //        NavigateUri = new Uri(href),
                        //        Content = uiContent,
                        //        Margin = new Thickness(-12),
                        //        Padding = new Thickness(0)
                        //    };
                        //hyperlinkButton.Padding = new Thickness(0);
                        //hyperlinkButton.Margin = new Thickness(0);

                        //result.Add(new InlineUIContainer()
                        //{
                        //    Child = hyperlinkButton
                        //});
                        result.Add(new Run() { Text = content });

                    }
                    catch (Exception exp)
                    {
                        result.Add(new Run() { Text = content });   
                    }
                }

                return true;
            }
            return false;
        }

        private string GetAttribute(string xml, string aName)
        {
            var s = aName + "=\"";
            var startIndex = xml.IndexOf(s);
            if (startIndex < 0)
            {
                return null;
            }
            var nextIndex = xml.IndexOf("\"", startIndex + s.Length + 1);
            if (nextIndex < 0)
            {
                return null;
            }

            var att = xml.Substring(startIndex + s.Length, nextIndex - startIndex - s.Length);
            return att;
        }

        private bool HandleParagraph(InlineCollection result, List<string> elements)
        {
            string startTag = "<p>";
            string endTag = "</p>";
            if (!elements.Any())
            {
                return false;
            }

            var item = elements.First();
            if (item.Equals(startTag, StringComparison.OrdinalIgnoreCase))
            {
                elements.RemoveAt(0);
                var inElements = GetElements(elements, startTag, endTag);
                if (result.Count > 0 && !(result.Last() is LineBreak))
                    elements.Insert(0, "<br/>");
                elements.InsertRange(0, inElements);
                elements.Insert(0, "<br/>");
                return true;
            }
            return false;
        }

        private bool HandleSimpleTags<TInline>(InlineCollection result, List<string> elements, string tagName) where TInline : Span, new()
        {
            string startTag = "<" + tagName + ">";
            string endTag = "</" + tagName + ">";
            if (!elements.Any())
            {
                return false;
            }

            var item = elements.First();
            if (item.Equals(startTag, StringComparison.OrdinalIgnoreCase))
            {
                elements.RemoveAt(0);
                var inElements = GetElements(elements, startTag, endTag);
                var bold = new TInline();
                GetInlines(bold.Inlines, inElements);
                result.Add(bold);
                return true;
            }
            return false;
        }

        private List<string> GetElements(List<string> elements, string startTag, string endTag)
        {
            var k = 1;
            var result = new List<string>();
            while (elements.Any())
            {
                if (elements.First().StartsWith(startTag, StringComparison.OrdinalIgnoreCase))
                {
                    k++;
                }

                if (elements.First().Equals(endTag, StringComparison.OrdinalIgnoreCase))
                {
                    k--;
                    if (k == 0)
                    {
                        elements.RemoveAt(0);
                        return result;
                    }
                }
                if (!String.IsNullOrEmpty(elements.First()))
                {
                    result.Add(elements.First());
                }
                elements.RemoveAt(0);
            }

            return result;
        }



        private bool HandleBR(InlineCollection result, List<string> elements)
        {
            if (!elements.Any())
            {
                return false;
            }
            var item = elements.First();
            if (IsBr(item))
            {
                result.Add(new LineBreak());
                elements.RemoveAt(0);
                while (elements.Count > 0 && (IsBr(elements.First()) || String.IsNullOrWhiteSpace(elements.First())))
                {
                    elements.RemoveAt(0);
                }
                if (elements.Count > 0)
                {
                    elements[0] = elements[0].TrimStart();

                }
                return true;
            }
            return false;
        }

        private bool IsBr(string item)
        {
            return item.Equals("<br/>", StringComparison.OrdinalIgnoreCase) ||
                   item.Equals("<br />", StringComparison.OrdinalIgnoreCase);
        }

        private bool HandleNOBR(InlineCollection result, List<string> elements)
        {
            if (!elements.Any())
            {
                return false;
            }
            var item = elements.First();
            if (IsNOBR(item))
            {
                result.Add(" ");
                elements.RemoveAt(0);
                while (elements.Count > 0 && (IsNOBR(elements.First()) || String.IsNullOrWhiteSpace(elements.First())))
                {
                    elements.RemoveAt(0);
                }
                if (elements.Count > 0)
                {
                    elements[0] = elements[0].TrimStart();

                }
                return true;
            }
            return false;
        }

        private bool IsNOBR(string item)
        {
            return item.Equals("<nobr/>", StringComparison.OrdinalIgnoreCase)
                    || item.Equals("<nobr>", StringComparison.OrdinalIgnoreCase);
        }


        private bool IsImage(string item)
        {
            return item.StartsWith("<img", StringComparison.OrdinalIgnoreCase);
        }

        private string GetImageSource(string item)
        {
            return GetAttribute(item, "src");
        }

        private bool HandleImage(InlineCollection result, List<string> elements)
        {
            if (!elements.Any())
            {
                return false;
            }
            var item = elements.First();
            if (IsImage(item))
            {
                var attr = GetImageSource(item);
                if (attr != null)
                {
                    var bitmapImage = new BitmapImage(new Uri(attr));

                    var image = new Image()
                    {
                        Source = bitmapImage
                    };
                    //image.ImageOpened += (s, e) =>
                    //{
                    //    image.InvalidateMeasure();
                    //    image.InvalidateArrange();
                    //};
                    image.Margin = new Thickness(0, 12, 0, 12);
                    while (result.Count > 0 && result.Last() is LineBreak)
                    {
                        result.Remove(result.Last());
                    }


                    result.Add(new InlineUIContainer()
                    {
                        Child = image,
                    });



                }
                elements.RemoveAt(0);
                var isBlRemoved = false;
                do
                {
                    isBlRemoved = false;
                    if (elements.Count > 0)
                    {
                        var first = elements.First().Trim();
                        if (String.IsNullOrWhiteSpace(first) || first.Equals("<br/>"))
                        {
                            elements.RemoveAt(0);
                            isBlRemoved = true;
                        }
                    }
                } while (isBlRemoved);
                return true;
            }
            return false;
        }

        private bool HandleText(ICollection<Inline> result, List<string> elements)
        {
            if (!elements.Any())
            {
                return false;
            }

            if (!IsTag(elements.First()))
            {
                //var txt = WebUtility.HtmlDecode(elements.First());
                var txt =  System.Net.HttpUtility.HtmlDecode(elements.First());
                //var txt = elements.First().Trim();
                if (txt.Equals(""))
                {
                    elements.RemoveAt(0);
                    return true;
                }
                if (txt.Equals("\r\n") || txt.Equals("\\r\\n"))
                {
                    result.Add(new LineBreak());
                }
                else
                {
                    result.Add(new Run() { Text = txt });
                }

                elements.RemoveAt(0);
                return true;
            }
            return false;
        }

        private bool IsTag(string item)
        {
            return item.StartsWith("<");
        }
    }

    public class HtmlToParagraphConvertor
    {
        public Paragraph GetParagraps(string html)
        {
            var paragraph = new Paragraph();
            //paragraph.LineStackingStrategy = LineStackingStrategy.MaxHeight;
            paragraph.TextAlignment = TextAlignment.Left;
            html = html.Replace("\r\n ", "<br/>").Replace("\r\n", "<br/>").Replace("\r", "").Replace("\n", "");
            if (String.IsNullOrWhiteSpace(html))
            {
                return paragraph;
            }
            var charArray = html.ToCharArray();
            var elements = GetElements(charArray);
            new ListArrayToHtml().GetInlines(paragraph.Inlines, elements);

            return paragraph;
        }



        private List<string> GetElements(IEnumerable<char> charArray)
        {
            var elements = new List<string>();
            var sb = new StringBuilder();
            foreach (var c in charArray)
            {
                if (!IsTag(c))
                {
                    sb.Append(c);
                }
                else
                {
                    if (IsStartTag(c))
                    {
                        elements.Add(sb.ToString());
                        sb = new StringBuilder();
                        sb.Append(c);
                    }
                    if (IsEndTag(c))
                    {
                        sb.Append(c);
                        elements.Add(sb.ToString());
                        sb = new StringBuilder();
                    }
                }
            }

            if (sb.Length != 0)
            {
                elements.Add(sb.ToString());
            }
            return elements;
        }

        private bool IsTag(char c)
        {
            return c == '<' || c == '>';
        }

        private bool IsStartTag(char c)
        {
            return c == '<';
        }

        private bool IsEndTag(char c)
        {
            return c == '>';
        }
    }

}