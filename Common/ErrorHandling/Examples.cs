namespace Common.ErrorHandling
{
    class Examples
    {
        // Recover from an error
        void Usage1()
        {
            try
            {
                File.ReadAllLines("File not there");
            }
            catch (Exception)
            {
                // Read from back file instead
            }

            // Normal flow continue
        }

        // Convert to a user/developer friendly coded error, only for the purpose
        // 1. Preserve details
        // 2. Can be translated later
        void Usage2()
        {
            try
            {
                // ServiceA.SaveShipment(xxx)
            }
            catch (Exception ex)
            {
                // Wrap the exception with code and detail
                throw ex.Bag("ProcessorService.SaveShipment", new { Reference = 123 });
            }
        }

        // Otherwise just let it fail, do not use try-catch block to swallow exceptions!
        void Usage3()
        { 
            // Database IO operation that may 
        }
    }
}
