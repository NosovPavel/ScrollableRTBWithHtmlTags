ScrollableRichTextBoxWithHtmlTags
=================================

Этот проект поможет отобразить длинный текст в приложении для Windows Phone 7\8 с html вставками в одном элеменете. Прична по которой был создан этот проект - ограничение по высоте элементов в Windows Phone 7\8. 

<h1>Как использовать:</h1>
1) Склонировать
2) Открыть в VS
3) Запустить и потестировать
4) Собранную dll из папки (ScrollableRTBWithHtmlTags\Phone.Controls\Bin\Debug(Release)\Phone.Controls.dll) добавить в свой проект
5) Добавить ссылку на Phone.Controls.dll в свой проект
6) Добавить пространство имен в .xaml файл xmlns:my="clr-namespace:Phone.Controls;assembly=Phone.Controls"
7) Добавить следующий код в .xaml файл страницы:
<code>         
	<Grid x:Name="ContentPanel" Grid.Row="1" Margin="12,0,12,0">
     <my:ScrollableTextBlock            
     HorizontalAlignment="Left"  Name="scrollableTextBlock1" 
     VerticalAlignment="Top"/>
	</Grid>
</code>
8) В .cs файл страницы добавить scrollableTextBlock1.Text = text; (вместо переменной text передаем сюда свой html код)

Этот проект является объединением нескольких проектов созданных Александр Краковецким и Ахмедом Шериевым и дополенным автором этих строк. 

Вопросы и пожелания пишите на почту NosovPavel2005@yandex.ru