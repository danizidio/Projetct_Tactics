using ShowText;

public class TextUI : TextManager
{
    public void CallText()
    {
        OnCallText?.Invoke();
    }
}
