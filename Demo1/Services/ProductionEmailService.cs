namespace Demo1.Services {
    public class ProductionEmailService : IEmailService {
        private string _to;
        private string _from;

        public ProductionEmailService() {
            _to = "simon@real.com";
            _from = "api@real.com";
        }

        public void Send(string subject, string message) {
            Console.WriteLine("--- REAL EMAIL!!! ---------------------------");
            Console.WriteLine($"To: {_to}");
            Console.WriteLine($"From: {_from}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
            Console.WriteLine("------------------------------");
        }
    }
}
