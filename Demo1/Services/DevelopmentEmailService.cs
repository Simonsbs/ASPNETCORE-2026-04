namespace Demo1.Services {
    public class DevelopmentEmailService {
        private string _to;
        private string _from;

        public DevelopmentEmailService() {
            _to = "simon@email.com";
            _from = "api@email.com";
        }

        public void SendDevEmail(string subject, string message) {
            Console.WriteLine("------------------------------");
            Console.WriteLine($"To: {_to}");
            Console.WriteLine($"From: {_from}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
            Console.WriteLine("------------------------------");
        }
    }
}
