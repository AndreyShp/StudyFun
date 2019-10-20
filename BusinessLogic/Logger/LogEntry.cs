namespace BusinessLogic.Logger {
    public class LogEntry {
        public long Id { get; set; }
        public byte MessageType { get; set; }
        public string Message { get; set; }
    }
}