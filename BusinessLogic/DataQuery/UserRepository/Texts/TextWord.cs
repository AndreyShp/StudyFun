namespace BusinessLogic.DataQuery.UserRepository.Texts {
    public class TextWord {
        public TextWord() {
            StartPosInText = -1;
        }

        public string Word { get; set; }
        public int StartPosInText { get; set; }
        public int EndPosInText { get; set; }
        
        public void AddCharToWord(char ch, int index) {
            Word += ch;
            if (StartPosInText > 0) {
                StartPosInText = index;
            }
            EndPosInText = index;
        }
    }
}