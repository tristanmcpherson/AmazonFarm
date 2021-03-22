using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AmazonFarm
{
    public class Selenium : ConsoleHelper
    {
        private readonly Config config;
        private readonly IWebDriver webDriver;
        private readonly AsinGroup asinGroup;

        public Selenium(Config config, AsinGroup asinGroup) {
            var options = new ChromeOptions {
                PageLoadStrategy = PageLoadStrategy.Default,
            };

            webDriver = new ChromeDriver(options);
            webDriver.Manage().Cookies.DeleteAllCookies();
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            this.config = config;
            this.asinGroup = asinGroup;
        }

        public void Start(CancellationToken token) {
            Login();

            while (!token.IsCancellationRequested) {
                foreach (var asin in asinGroup.Asins) {
                    var checkingOut = CheckForItems(asin, asinGroup);
                    if (!checkingOut) {
                        continue;
                    }

                    if (token.IsCancellationRequested) {
                        return;
                    }

                    ContinueCheckout();
                    return;
                }
            }
        }

        public void ContinueCheckout() {
            // Switch to checkout iFrame
            var iFrame = webDriver.FindElement(By.Id("turbo-checkout-iframe"));
            webDriver.SwitchTo().Frame(iFrame);
            webDriver.FindElement(By.Id("turbo-checkout-place-order-button")).Click();
            webDriver.SwitchTo().ParentFrame();

            Green("Purchased item! Congratz! Goodbye.");
        }

        public void Stop() {
            webDriver.Quit();
        }

        public void Login() {
            webDriver.Url = $"https://{config.Domain}/";

            webDriver.FindElement(By.XPath("//*[@id=\"ge-hello\"]/div[2]/span/a")).Click();
            webDriver.FindElement(By.Id("ap_email")).SendKeys(config.Email);
            webDriver.FindElement(By.Id("continue")).Click();
            webDriver.FindElement(By.Id("ap_password")).SendKeys(config.Password);
            webDriver.FindElement(By.Id("signInSubmit")).Click();
        }

        public bool CheckForItems(string asin, AsinGroup asinGroup) {
            Console.WriteLine($"Checking price of {asin}");

            webDriver.Url = $"https://{config.Domain}/dp/{asin}";

            // Find first element: out of stock, price, other offers only
            var element = webDriver.FindElements(
                By.XPath("//div[@id='outOfStock'] | //span[@id='price_inside_buybox'] | //span[@data-action='show-all-offers-display']")
            ).FirstOrDefault();

            if (element == null) {
                Console.WriteLine("UNEXPECTED STATE.");
                return false;
            }

            var id = element.GetAttribute("id");
            if (id == "outOfStock") {
                Red("Out of stock.");
                return false;
            }

            if (id == "price_inside_buybox") {
                return CheckPrice(element, asinGroup);
            }

            if (element.GetAttribute("data-action") == "show-all-offers-display") {
                Red("Only other offers.");
                return false;
            }

            return false;
        }

        private static readonly Regex priceRegex = new Regex(@"\$(?<dollars>\d+)\.\d+");

        bool CheckPrice(IWebElement element, AsinGroup asinGroup) {
            var priceText = priceRegex.Match(element.Text.Replace(",", "")).Groups["dollars"].Value;
            var price = Convert.ToInt32(priceText);

            if (price <= asinGroup.MaxPrice && price >= asinGroup.MinPrice) {
                Green($"Price ${price}. Purchasing item!!!");

                webDriver.FindElement(By.Id("buy-now-button")).Click();
                return true;
            }

            Red($"Price invalid: ${price}");
            return false;
        }
    }
}
