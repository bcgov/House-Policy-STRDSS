namespace UITest.TestEngine
{
    public class Engine
    {

        public void ExecuteTest(List<Func<bool>> Actions)
        {
            foreach (var action in Actions)
            {
                ExecuteAction(action);
            }
        }

        bool ExecuteAction(Func<bool> TestAction)
        {
            bool result = false;
            try
            {
                result = TestAction();
            }
            catch (Exception ex)
            {
                ///TODO: Log exception

                throw;
            }

            return (true);
        }
    }
}
