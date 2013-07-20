ScrollableRichTextBoxWithHtmlTags
=================================

Этот проект поможет отобразить длинный текст в приложении для Windows Phone 7\8 с html вставками в одном элеменете. Прична по которой был создан этот проект - ограничение по высоте элементов в Windows Phone 7\8. 

<h1>Как использовать:</h1>
<ol style="text-align: left;">
<li>Склонировать</li>
<li>Открыть в VS</li>
<li>Запустить и потестировать</li>
<li>Собранную dll из папки (ScrollableRTBWithHtmlTags\Phone.Controls\Bin\Debug(Release)\Phone.Controls.dll) добавить в свой проект</li>
<li>Добавить ссылку на Phone.Controls.dll в свой проект</li>
<li>Добавить пространство имен в .xaml файл xmlns:my="clr-namespace:Phone.Controls;assembly=Phone.Controls"</li>
<li>Добавить следующий код в .xaml файл страницы:<br />&lt;grid grid.row="1" margin="12,0,12,0" x:name="ContentPanel"&gt;<br />&lt;my:scrollabletextblock horizontalalignment="Left" name="scrollableTextBlock1" verticalalignment="Top"&gt;<br />&lt;/my:scrollabletextblock&gt;&lt;/grid&gt;</li>
<li>В .cs файл страницы добавить scrollableTextBlock1.Text = text; (вместо переменной text передаем сюда свой html код)</li>
</ol>

Этот проект является объединением нескольких проектов созданных Александр Краковецким и Ахмедом Шериевым и дополенным автором этих строк. 

Вопросы и пожелания пишите на почту NosovPavel2005@yandex.ru