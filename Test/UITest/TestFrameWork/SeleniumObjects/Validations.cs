using UITest.TestDriver;

namespace UITest.SeleniumObjects
{
    public class Validations
    {
        private List<Validation> _ValidationList;
        private IDriver _Driver;

        public List<Validation> ValidationList
        {
            get
            {
                return _ValidationList;
            }

            set
            {
                _ValidationList = value;
            }
        }

        public Validations(IDriver Driver)
        {
            _Driver = Driver;
            ValidationList = new List<Validation>();
        }

        public bool Validate()
        {
            foreach (var validation in _ValidationList)
            {
                _Driver.WaitFor(validation.LocatorType, validation.Locator, validation.WaitTime);
                _Driver.FindElement(validation.LocatorType, validation.Locator);
            }
            return (true);
        }

    }

    public class Validation
    {
        private Enums.FINDBY _locatorType = Enums.FINDBY.ID;
        private string _Locator = string.Empty;
        private int _WaitTime = 1;

        public Enums.FINDBY LocatorType
        {
            get
            {
                return _locatorType;
            }

            set
            {
                _locatorType = value;
            }
        }

        public string Locator
        {
            get
            {
                return _Locator;
            }

            set
            {
                _Locator = value;
            }
        }

        public int WaitTime
        {
            get
            {
                return _WaitTime;
            }
        }

        public Validation(string Locator, int WaitTime = 1)
        {
            _Locator = Locator;
            _WaitTime = WaitTime;
        }
    }
}
