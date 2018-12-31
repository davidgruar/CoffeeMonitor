namespace CoffeeMonitor.Model.Messaging
{
    public class DurableFunctionInstanceData
    {
        public string Id { get; set; }

        public string StatusQueryGetUri { get; set; }

        public string SendEventPostUri { get; set; }

        public string TerminatePostUri { get; set; }

        public string RewindPostUri { get; set; }
    }
}