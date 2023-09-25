namespace tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Requirement : Attribute
    {
        public int Number;
        public Requirement(int number)
        {
            Number = number;
        }
    }
}