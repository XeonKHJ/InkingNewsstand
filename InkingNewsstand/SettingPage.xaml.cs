using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace InkingNewsstand
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            isInitializing = true;
            this.InitializeComponent();
            FontSizeSlider.Value = Settings.FontSize;
            LineSpacingSlider.Value = Settings.LineSpacing;
            PageWidthSlider.Value = Settings.NewsWidth;
            FontButton.Content = Settings.Font.Source;
            foreach (var font in fontNames)
            {
                var menuFlyoutItem = new MenuFlyoutItem() { Text = font };
                menuFlyoutItem.Click += MenuFlyoutItem_Click; ;
                FontFlyout.Items.Add(menuFlyoutItem);
            }
            isInitializing = false;
        }
        private bool isInitializing = false;
        string Html = "Dependency Autocompletion, Performance Improvements and More for Java on Visual Studio Codehttps://devblogs.microsoft.com/visualstudio/java-on-visual-studio-code-february-update-dependency-autocompletion-performance-and-more/https://devblogs.microsoft.com/visualstudio/java-on-visual-studio-code-february-update-dependency-autocompletion-performance-and-more/#commentsTue, 26 Feb 2019 20:00:52 GMT2019-02-26T20:00:52Z2019-02-26T20:00:52ZXiaokai HeXiaokai HeOpen SourceVisual StudioAnnouncementDebuggingExtensionsIntelliCodeJavaMavenPerformanceRefactoringTestingVisual Studio Codehttps://devblogs.microsoft.com/visualstudio/?p=224354<p>Welcome to February update of Java on Visual Studio Code! We’d like to share a few new improvements to further enhance your productivity, including</p>\n<ul>\n<li>Dependency auto-completion and more Maven updates</li>\n<li>Performance improvements</li>\n<li>Standalone file supports</li>\n<li>Multiple source folders support</li>\n<li>Easy launch for multi-main-class projects</li>\n<li>Hide temporary files</li>\n<li>Bulk generate getters and setters</li>\n<li>Test configuration and report update</li>\n<li>Including IntelliCode to Java Extension Pack</li>\n</ul>\n<p>Try these new features by installing <a href=\"https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-java-pack\" target=_blank rel=noopener>Java Extension Pack</a> with Visual Studio Code. See below for more details!</p>\n<h2>Managing your Maven Dependencies Easily</h2>\n<p>Editing <em>pom.xml&#160;</em>is a common task for developer when working with Maven project. To make it easier with Visual Studio Code, the&#160;<a href=\"https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-maven\" target=_blank rel=noopener>Maven</a> extension now supports code snippet as well as dependency auto-completion. The extension pulls plugin information from your local repository as well as Maven Central to help you choose artifacts and versions as you type.</p>\n<p><img class=\"alignnone size-full wp-image-224406\" src=\"https://devblogs.microsoft.com/visualstudio/wp-content/uploads/sites/4/2019/02/pom-dependency-completion.gif\" alt=\"\" width=1024 height=768></p>\n<p>Maven plugins and their goals are now listed in the explorer along with other Maven resources. You can execute the goals with just a few quick clicks.</p>\n<p><img class=\"alignnone size-full wp-image-224407\" src=\"https://devblogs.microsoft.com/visualstudio/wp-content/uploads/sites/4/2019/02/maven-plugin-goals.png\" alt=\"\" width=630 height=578></p>\n<p>Maven explorer now also allows you to switch between flat view and hierarchical view, to your preference.</p>\n<h2>Loading Extensions and Projects Faster</h2>\n<p>One of the key performance related scenarios we’re improving for editor performance is loading projects. Two improvements are introduced with this update.</p>\n<ol>\n<li>The load time of Java extensions is reduced by adopting <em>webpack</em>. All Visual Studio Code extensions are written in JavaScript/TypeScript. Recently, we started to adopt <em>webpack</em> to generate the production package, with code combined and minified. This dramatically reduced extension load time. Please update the extensions to the newest version, so you will get this improvement automatically.</li>\n<li>Thanks to the improvement made from upstream JDT project, we can now enable parallel build in Java Language Server. By doing so, the time of loading a project can be reduced. The build process is per project. You will get the most performance gain when you have multiple child projects in your workspace. To enable parallel build, open <em>setting.json</em> and set the option <em>java.maxConcurrentBuilds</em> to a numeric value. The recommended value is the number of CPU cores on your machine.<br>\n<pre class=crayon-plain-tag>{ \n    &quot;java.maxConcurrentBuilds&quot;: 4 // on a 4-core machine \n}</pre>\n</li>\n</ol>\n<h2>Handling All Your Source Code</h2>\n<p>If you want to work with java files directly but don’t bother to create a project, we’ve now got you covered with improved standalone Java file support.</p>\n<p>The solution is folder based, so all you need to do is open a folder with Visual Studio Code and all the Java files within the folder will be properly compiled. Then you will be free to run or debug them.</p>\n<p><img class=\"alignnone size-full wp-image-224397\" src=\"https://devblogs.microsoft.com/visualstudio/wp-content/uploads/sites/4/2019/02/standalone.file_.support.gif\" alt=\"\" width=1024 height=768></p>\n<p>What if you have multiple sub-folders that have source code inside and want to have them handled correctly? Just add these folders to source path, then all the code inside those folders will be correctly compiled.</p>\n<p><img class=\"alignnone size-full wp-image-224398\" src=\"https://devblogs.microsoft.com/visualstudio/wp-content/uploads/sites/4/2019/02/multiple.source.folder.gif\" alt=\"\" width=1024 height=768></p>\n<p>If you have&#160;multiple main classes in your workspace, you can have a special launch configuration to launch whatever is in the active editor. This comes handy when you use hot keys. Here’s the new configuration:</p><pre class=crayon-plain-tag>{ \n    &quot;type&quot;: &quot;java&quot;, \n    &quot;name&quot;: &quot;Debug (Launch) - Current File&quot;, \n    &quot;request&quot;: &quot;launch&quot;, \n    &quot;mainClass&quot;: &quot;${file}&quot; // whatever main class in the active editor \n}</pre><p><img class=\"alignnone size-full wp-image-224409\" src=\"https://devblogs.microsoft.com/visualstudio/wp-content/uploads/sites/4/2019/02/multiple.main_.entry_.gif\" alt=\"\" width=1024 height=768></p>\n<h2>Hide Temporary Files</h2>\n<p>After opening a project folder, some extra files will be generated by Java Language Server inside the folder to work properly. Now you can choose to hide those files in Visual Studio Code. When opening a project folder, Java Language Server will ask you how you would like to handle those temporary files. You can hide them globally or just within the current workspace or leave them as-is.</p>\n<p><img class=\"alignnone size-full wp-image-224399\" src=\"https://devblogs.microsoft.com/visualstudio/wp-content/uploads/sites/4/2019/02/hide.temp_.files_.png\" alt=\"\" width=898 height=234></p>\n<h2>Bulk Generate Getters &amp; Setters</h2>\n<p>More source actions were added to the language server. Now you can bulk generate getters and setters for all new member variables.</p>\n<h2><img class=\"alignnone size-full wp-image-224408\" src=\"https://devblogs.microsoft.com/visualstudio/wp-content/uploads/sites/4/2019/02/bulk.gen_.getter.setter.gif\" alt=\"\" width=1024 height=768></h2>\n<h2>Easier Test Configuration and Report Navigation</h2>\n<p>Test configurations are very useful in special test setups. Those configurations were originally stored in <em>launch.test.json</em>, which generated lots of confusions according to the user feedback. We listened, and as a result, we <strong>deprecated</strong> <em>launch.test.json</em>, and replaced it with regular VS Code settings. Now, the test configurations stay in&#160;<em>settings.json</em>, which can be global or at the workspace level. And they look like this</p><pre class=crayon-plain-tag>&quot;java.test.config&quot;: [ \n    { \n        &quot;name&quot;: &quot;myTestConfiguration&quot;, \n        &quot;workingDirectory&quot;: &quot;${workspaceFolder}&quot;, \n        &quot;args&quot;: [ &quot;-c&quot;, &quot;com.test&quot; ], \n        &quot;vmargs&quot;: [ &quot;-Xmx512M&quot; ], \n        &quot;env&quot;: { &quot;key&quot;: &quot;value&quot; }, \n    }, \n    { \n        // Another configuration entry... \n    } \n]</pre><p>For more details, visit <a href=\"https://github.com/Microsoft/vscode-java-test/blob/master/runner-config.md\">Test Runner Configuration</a>.</p>\n<p>The updated test report now allows you to jump to the definition of a test case directly by click the link in the reports.</p>\n<p><img class=\"alignnone size-full wp-image-224410\" src=\"https://devblogs.microsoft.com/visualstudio/wp-content/uploads/sites/4/2019/02/navigate.to_.source.gif\" alt=\"\" width=1024 height=768></p>\n<h2>Including IntelliCode to Java Extension Pack</h2>\n<p>As we’ve introduced in our previous <a href=\"https://devblogs.microsoft.com/visualstudio/ai-assisted-coding-comes-to-java-with-visual-studio-intellicode/\">blog</a>, IntelliCode saves you time by putting what you’re most likely to use at the top of your completion list. After releasing it 3 months ago, we see more and more developers adopting it with great feedbacks. Thus we decided to include it into our&#160;<a href=\"https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-java-pack\" target=_blank rel=noopener>Java Extension Pack</a> so more developers could benefit from AI-assisted coding.</p>\n<p><img class=\"alignnone size-full wp-image-224396\" src=\"https://devblogs.microsoft.com/visualstudio/wp-content/uploads/sites/4/2019/02/intellicode.gif\" alt=\"\" width=1024 height=768></p>\n<h2>Try it out</h2>\n<p>Please don’t hesitate to give it a try! Your feedback and suggestions are very important to us and will help shape our product in future. You may take this&#160;<a href=\"https://www.research.net/r/vscodejava-blog?o=%5bo_value%5d&amp;m=%5bm_value%5d\" target=_blank rel=noopener>survey</a> to share your thoughts!</p>\n<p>Visual Studio Code is a fast and lightweight code editor with great Java support from many extensions</p>\n<ul>\n<li><a href=\"https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-java-pack\" target=_blank rel=noopener>Java Extension Pack</a> includes essential Java tools including <a href=\"https://marketplace.visualstudio.com/items?itemName=redhat.java\" target=_blank rel=noopener>Language Support for Java<img src=\"https://s.w.org/images/core/emoji/11/72x72/2122.png\" alt=\"™\" class=wp-smiley style=\"height:1em;max-height:1em;\"> by Red Hat</a>, <a href=\"https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-java-debug\" target=_blank rel=noopener>Debugger for Java</a>, <a href=\"https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-maven\" target=_blank rel=noopener>Maven</a>, <a href=\"https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-java-test\" target=_blank rel=noopener>Java Test Runner</a> and <a href=\"https://go.microsoft.com/fwlink/?linkid=2006060\" target=_blank rel=noopener>IntelliCode Extension for Visual Studio Code</a>.</li>\n<li>There’re also other Java related extensions you can choose from, including\n<ul>\n<li><a href=\"https://marketplace.visualstudio.com/items?itemName=adashen.vscode-tomcat\" target=_blank rel=noopener>Tomcat</a> and <a href=\"https://marketplace.visualstudio.com/items?itemName=SummerSun.vscode-jetty\" target=_blank rel=noopener>Jetty</a> for quickly deploy and manage local app servers.</li>\n<li>In case you’re working on Spring Boot, there’re also great support provided by <a href=\"https://marketplace.visualstudio.com/items?itemName=Pivotal.vscode-boot-dev-pack\" target=_blank rel=noopener>Pivotal</a> and <a href=\"https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-spring-initializr\" target=_blank rel=noopener>Microsoft</a> available on Visual Studio Code including <a href=\"https://marketplace.visualstudio.com/items?itemName=Pivotal.vscode-spring-boot\" target=_blank rel=noopener>Spring Boot Tools</a>, <a href=\"https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-spring-initializr\" target=_blank rel=noopener>Spring Initializr</a> and <a href=\"https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-spring-boot-dashboard\" target=_blank rel=noopener>Spring Boot Dashboard</a>.</li>\n<li><a href=\"https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-java-dependency\" target=_blank rel=noopener>Java Dependencies</a> provides you a package view of your Java project and helps you managing your dependencies.</li>\n<li><a href=\"https://marketplace.visualstudio.com/items?itemName=shengchen.vscode-checkstyle\" target=_blank rel=noopener>Checkstyle</a> could be handy when you need coherence code style especially cross multiple team members.</li>\n</ul>\n</li>\n<li>Learn more about <a href=\"https://code.visualstudio.com/docs/languages/java\" target=_blank rel=noopener>Java on Visual Studio Code</a>.</li>\n<li>Explore our step by step <a href=\"https://code.visualstudio.com/docs/java/java-tutorial\" target=_blank rel=noopener>Java Tutorials on Visual Studio Code</a>.</li>\n</ul>\n<p>The post <a rel=nofollow href=\"https://devblogs.microsoft.com/visualstudio/java-on-visual-studio-code-february-update-dependency-autocompletion-performance-and-more/\">Dependency Autocompletion, Performance Improvements and More for Java on Visual Studio Code</a> appeared first on <a rel=nofollow href=\"https://devblogs.microsoft.com/visualstudio\">The Visual Studio Blog</a>.</p>\n<p>Welcome to February update of Java on Visual Studio Code! We’d like to share a few new improvements to further enhance your productivity, including</p>\n<ul>\n<li>Dependency auto-completion and more Maven updates</li>\n<li>Performance improvements</li>\n<li>Standalone file supports</li>\n<li>Multiple source folders support</li>\n<li>Easy launch for multi-main-class projects</li>\n<li>Hide temporary files</li>\n<li>Bulk generate getters and setters</li>\n<li>Test configuration and report update</li>\n<li>Including IntelliCode to Java Extension Pack</li>\n</ul>\n<p>Try these new features by installing <a href=\"https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-java-pack\" target=_blank rel=noopener>Java Extension Pack</a> with Visual Studio Code.</p>\n<p>The post <a rel=nofollow href=\"https://devblogs.microsoft.com/visualstudio/java-on-visual-studio-code-february-update-dependency-autocompletion-performance-and-more/\">Dependency Autocompletion, Performance Improvements and More for Java on Visual Studio Code</a> appeared first on <a rel=nofollow href=\"https://devblogs.microsoft.com/visualstudio\">The Visual Studio Blog</a>.</p>\nhttps://devblogs.microsoft.com/visualstudio/java-on-visual-studio-code-february-update-dependency-autocompletion-performance-and-more/feed/1";

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Settings.Font = new FontFamily(((MenuFlyoutItem)sender).Text);
            FontButton.Content = ((MenuFlyoutItem)sender).Text;
            ValueChanged?.Invoke(sender, null);
        }

        private string[] fontNames = Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies();

        public static event RangeBaseValueChangedEventHandler ValueChanged;
        private void FontSizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Settings.FontSize = e.NewValue;
            ValueChanged?.Invoke(sender, e);
        }

        private void LineSpacingSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Settings.LineSpacing = e.NewValue;
            ValueChanged?.Invoke(sender, e);
        }

        private void PageWidthSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!isInitializing)
            {
                Settings.NewsWidth = e.NewValue;
            }
            ValueChanged?.Invoke(sender, e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Settings.SaveSettings();
        }

        private void BindingWindowCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(bindingWindowCheckBox.IsChecked.ToString());
            Settings.BindingNewsWidthwithFrame = true;
            OnBindingWindowCheckBoxChanged?.Invoke(bindingWindowCheckBox);
        }
        public delegate void OnBindingWindowCheckBoxChangedHandler(CheckBox sender);
        public static event OnBindingWindowCheckBoxChangedHandler OnBindingWindowCheckBoxChanged;

        private void BindingWindowCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(bindingWindowCheckBox.IsChecked.ToString());
            Settings.BindingNewsWidthwithFrame = false;
            OnBindingWindowCheckBoxChanged?.Invoke(bindingWindowCheckBox);
        }
    }
}
