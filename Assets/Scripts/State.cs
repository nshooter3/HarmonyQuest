public class State
{
    public string name;

    public delegate void EnterDelegate();
    public EnterDelegate Enter;
    public delegate void UpdateDelegate();
    public UpdateDelegate Update;
    public delegate void ExitDelegate();
    public ExitDelegate Exit;

    public State(string name, EnterDelegate enterFunc, UpdateDelegate updateFunc, ExitDelegate exitFunc)
    {
        this.name = name;
        Enter += enterFunc;
        Update += updateFunc;
        Exit += exitFunc;
    }
}
