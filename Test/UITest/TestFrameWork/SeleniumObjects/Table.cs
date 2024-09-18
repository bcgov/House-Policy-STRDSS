using OpenQA.Selenium;
using UITest.TestDriver;

namespace UITest.SeleniumObjects
{
    public class Table : UIElement
    {
        public string TableLocator { get; set; }
        public IDriver _Driver = null;

        public Table(IDriver Driver, Enums.FINDBY LocatorType, string Locator) : base(Driver)
        {
            base.Locator = Locator;
            base.LocatorType = LocatorType;
            _Driver = Driver;
        }

        public bool EnterText(string Text)
        {
            FindElement(base.LocatorType, base.Locator);
            SendKeys(Text);
            return (true);
        }

        public string GetCell(int row, int column)
        {
            try
            {
                // Locate the table using the provided CSS selector
                var table = _Driver.FindElement(By.CssSelector(Locator));

                // Locate the specified row and column cell within the table
                var cell = table.FindElement(By.CssSelector($"tr:nth-child({row + 1}) td:nth-child({column + 1})"));

                // Return the text content of the cell
                return cell.Text;
            }
            catch (NoSuchElementException)
            {
                // Return null if the cell is not found
                return null;
            }
        }

        public List<string> GetRow(int row)
        {
            try
            {
                // Locate the table using the provided CSS selector
                var table = _Driver.FindElement(By.CssSelector(Locator));

                // Locate the specified row within the table
                var rowElement = table.FindElement(By.CssSelector($"tr:nth-child({row + 1})"));

                // Retrieve all cell elements (td) within the specified row
                var cells = rowElement.FindElements(By.TagName("td"));

                // Return the text content of each cell in the row as a list of strings
                return cells.Select(cell => cell.Text).ToList();
            }
            catch (NoSuchElementException)
            {
                // Return an empty list if the row is not found
                return new List<string>();
            }
        }

        public List<List<string>> GetAllTableData(string tableLocator)
        {
            try
            {
                // Locate the table
                var table = _Driver.FindElement(By.CssSelector(tableLocator));

                // Locate all rows within the table
                var rows = table.FindElements(By.CssSelector("tr"));
                var tableData = new List<List<string>>();

                // Loop through each row and get the data
                foreach (var row in rows)
                {
                    var cells = row.FindElements(By.TagName("td"));
                    var rowData = cells.Select(cell => cell.Text).ToList();

                    // Add row data to the table data list
                    if (rowData.Count > 0) // Only add non-empty rows
                    {
                        tableData.Add(rowData);
                    }
                }

                return tableData;
            }
            catch (NoSuchElementException)
            {
                // Return an empty list if the table is not found
                return new List<List<string>>();
            }
        }

        // Method to get the number of rows in the table
        public int GetRowCount(string tableLocator)
        {
            try
            {
                // Locate the table
                var table = _Driver.FindElement(By.CssSelector(tableLocator));

                // Count the number of rows
                var rows = table.FindElements(By.CssSelector("tr"));
                return rows.Count;
            }
            catch (NoSuchElementException)
            {
                // Return 0 if the table is not found
                return 0;
            }
        }

        // Method to get the number of columns in the first row of the table
        public int GetColumnCount(string tableLocator)
        {
            try
            {
                // Locate the table
                var table = _Driver.FindElement(By.CssSelector(tableLocator));

                // Locate the first row and count the number of columns (cells)
                var firstRow = table.FindElement(By.CssSelector("tr:nth-child(1)"));
                var columns = firstRow.FindElements(By.TagName("td"));
                return columns.Count;
            }
            catch (NoSuchElementException)
            {
                // Return 0 if the table or first row is not found
                return 0;
            }
        }

        public void EnterTextInCell(int row, int column, string text)
        {
            try
            {
                var table = _Driver.FindElement(By.CssSelector(Locator));
                var cell = table.FindElement(By.CssSelector($"tr:nth-child({row + 1}) td:nth-child({column + 1})"));

                // Find an input element inside the cell
                var inputField = cell.FindElement(By.CssSelector("input, textarea"));

                // Clear existing text (if any) and enter the new text
                inputField.Clear();
                inputField.SendKeys(text);
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine($"Element not found in the table at row {row + 1}, column {column + 1}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while entering text: {ex.Message}");
            }
        }
    }
}

