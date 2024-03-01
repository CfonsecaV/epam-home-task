using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace epam_home_task
{
    public class Tests
    {
        private IWebDriver driver;

        //Locators for ValidateCriteriaBasedPositionSearch
        private readonly By acceptCookiesButtonLocator = By.CssSelector("div button#onetrust-accept-btn-handler"); //Xpath
        private readonly By languageTextBoxLocator = By.Id("new_form_job_search-keyword"); //Id        
        private readonly By locationDropdownButtonLocator = By.CssSelector("span[role=presentation]"); //css
        private readonly By locationDropdownMenuLocator = By.CssSelector("ul[role] .os-content"); //css
        private readonly By careersTopMenuLocator = By.XPath("//a[@class='top-navigation__item-link js-op'][.='Careers']"); //Xpath
        private readonly By remoteCheckboxLocator = By.CssSelector("[name=remote]+label"); //css
        private readonly By findButtonLocator = By.XPath("//button[contains(text(), 'Find')]"); //Xpath
        private readonly By viewAndApplyButtonLocator = By.CssSelector("li.search-result__item:last-child .button-text"); //css
        private readonly By selectedLocationLocator = By.CssSelector(".select2-selection__rendered"); //css
        private string allLocationsOptionLocator = ".select2-results__option [title='{0}']"; //css



        //Locators for ValidateGlobalSearch
        private readonly By searchIconLocator = By.CssSelector("span.search-icon"); //css
        private readonly By searchTextBoxLocator = By.Name("q"); //Name
        private readonly By mainFindButtonLocator = By.XPath("//span[contains(text(), 'Find')]"); //Xpath
        private readonly By listElementLocator = By.CssSelector(".search-results__title-link"); //css
        private readonly By footerLocator = By.CssSelector("footer.search-results__footer"); //css

        private string? pageSource;

        private IWebElement GetAcceptCookiesButton() { return driver.FindElement(acceptCookiesButtonLocator); }
        private IWebElement GetLanguageTextBox() { return driver.FindElement(languageTextBoxLocator); }
        private IWebElement GetLocationDropdownButton() { return driver.FindElement(locationDropdownButtonLocator); }
        private IWebElement GetLocationDropdownMenu() { return driver.FindElement(locationDropdownMenuLocator); }
        private IWebElement GetCareersTopMenu() { return driver.FindElement(careersTopMenuLocator); }
        private IWebElement GetRemoteCheckbox() { return driver.FindElement(remoteCheckboxLocator); }
        private IWebElement GetFindButton() { return driver.FindElement(findButtonLocator); }
        private IWebElement GetViewAndApplyButton() { return driver.FindElement(viewAndApplyButtonLocator); }
        private IWebElement GetSelectedLocation() { return driver.FindElement(selectedLocationLocator); }
        private IWebElement GetAllLocationsOption(string option)
        {
            allLocationsOptionLocator = string.Format(allLocationsOptionLocator, option);
            return GetLocationDropdownMenu().FindElement(By.CssSelector(allLocationsOptionLocator));
        }

        private IWebElement GetSearchIcon() { return driver.FindElement(searchIconLocator); }
        private IWebElement GetSearchTextBox() { return driver.FindElement(searchTextBoxLocator); }
        private IWebElement GetMainFindButton() { return driver.FindElement(mainFindButtonLocator); }
        private IWebElement GetFooter() { return driver.FindElement(footerLocator); }
        private List<IWebElement> GetListElements() { return [.. driver.FindElements(listElementLocator)]; }

        private void AcceptCookies()
        {
            WaitCondition(() => GetAcceptCookiesButton().Enabled && GetAcceptCookiesButton().Displayed);
            GetAcceptCookiesButton().Click();
        }
        private void WaitCondition(Func<bool> condition)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.IgnoreExceptionTypes(typeof(ElementClickInterceptedException));
            try
            {
                wait.Until(d => condition());
            }
            catch (WebDriverTimeoutException) 
            {
                Console.WriteLine("Element was not found in time");
            }
            
        }
        private void ClickSearchIcon()
        {
            GetSearchIcon().Click();
        }
        private void InputSearchKeyword(string searchItem)
        {
            GetSearchTextBox().SendKeys(searchItem);
        }
        private void ClickMainFindButton()
        {
            GetMainFindButton().Click();
        }
        private void ClickCareerButton()
        {
            GetCareersTopMenu().Click();
        }
        private void InputProgrammingLanguage(string language)
        {
            GetLanguageTextBox().SendKeys(language);
        }
        private void ClickAllLocationsOption(string option)
        {
            MoveToLocationDropdownMenu(driver);
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)driver;
            javaScriptExecutor.ExecuteScript("arguments[0].scrollIntoView(true);", GetAllLocationsOption(option));
            GetAllLocationsOption(option).Click();
        }
        private void ClickRemoteCheckbox()
        {
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)driver;
            javaScriptExecutor.ExecuteScript("arguments[0].click();", GetRemoteCheckbox());
        }
        private void ClickFindButton()
        {
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)driver;
            javaScriptExecutor.ExecuteScript("arguments[0].click();", GetFindButton());
        }
        private void ClickViewAndApplyButton()
        {
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)(driver);
            javaScriptExecutor.ExecuteScript("arguments[0].scrollIntoView;", GetViewAndApplyButton());
            GetViewAndApplyButton().Click();
        }
        private void ClickLocationDropdownButton(IWebDriver driver)
        {
            Actions action = new(driver);
            action.MoveToElement(GetLocationDropdownButton());
            GetLocationDropdownButton().Click();
        }
        private void MoveToLocationDropdownMenu(IWebDriver driver)
        {
            Actions action = new(driver);
            action.MoveToElement(GetLocationDropdownMenu());
        }
        private void ScrollToFooter(IWebDriver driver)
        {
            Actions action = new(driver);
            action.ScrollToElement(GetFooter()).Perform();
        }
        private List<IWebElement> FilterList(string keyword)
        {
            var filteredElements = GetListElements()
                        .Where(element => element.GetAttribute("href")
                        .Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        .ToList();
            return filteredElements;
        }

        [SetUp]
        public void Setup()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            driver = new ChromeDriver(options);
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            var configuration = configBuilder.Build();
            string baseUrl = configuration["BaseUrl"] ?? string.Empty;
            driver.Navigate().GoToUrl(baseUrl);

        }

        [TearDown]
        public void TearDown()
        {
            driver.Dispose();
        }

        [Test]
        [TestCase("C#", "All Locations")]
        [TestCase("Java", "All Locations")]
        [TestCase("Python", "All Locations")]
        public void PageContainsInputProgrammingLanguage_DoesContainIt_Pass(string programmingLanguage, string location)
        {
            AcceptCookies();
            ClickCareerButton();
            InputProgrammingLanguage(programmingLanguage);
            ClickLocationDropdownButton(driver);
            ClickAllLocationsOption(location);

            Assert.That(GetSelectedLocation().Text, Does.Contain(location));

            WaitCondition(() => GetRemoteCheckbox().Displayed);
            ClickRemoteCheckbox();
            ClickFindButton();
            WaitCondition(() => GetViewAndApplyButton().Displayed);
            ClickViewAndApplyButton();
            pageSource = driver.PageSource;

            Assert.That(pageSource, Does.Contain(programmingLanguage));
        }

        [Test]
        [TestCase("BLOCKCHAIN")]
        [TestCase("Cloud")]
        [TestCase("Automation")]
        public void KeywordIsPresentInAllItemsOfList_IsNotPresentInAll_Fail(string keyword)
        {
            AcceptCookies();
            ClickSearchIcon();
            WaitCondition(() => GetSearchTextBox().Displayed);
            InputSearchKeyword(keyword);
            ClickMainFindButton();
            ScrollToFooter(driver);

            Assert.That(FilterList(keyword), Is.EqualTo(GetListElements())
                ,$"Filtered list amount({FilterList(keyword).Count}) " +
                $"is not the same as in Total list ({GetListElements().Count})");
        }
    }
}