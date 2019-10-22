public class State
{
    public delegate void EnterDelegate();
    public EnterDelegate Enter;
    public delegate void UpdateDelegate();
    public UpdateDelegate Update;
    public delegate void ExitDelegate();
    public ExitDelegate Exit;

    public State(EnterDelegate enterFunc, UpdateDelegate updateFunc, ExitDelegate exitFunc)
    {
        Enter += enterFunc;
        Update += updateFunc;
        Exit += exitFunc;
    }
}
