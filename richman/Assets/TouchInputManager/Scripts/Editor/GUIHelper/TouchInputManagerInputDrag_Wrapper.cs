namespace TouchInputManagerBackendEditor
{
    //Tangoing with one of Unity's many quirks.
    public class TouchInputManagerInputDrag_Wrapper
    {
    
        private object value;

        public object Value
        {
            get { return value; }
        }

        public TouchInputManagerInputDrag_Wrapper(object value)
        {
            this.value = value;
        }

    }
}