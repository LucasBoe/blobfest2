using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IFromToDataProvider
{ 
    bool TryGetFromToData(out FromToData data);
}
public class TokenFromToUIModule : DataUIModule<IFromToDataProvider>
{
    [SerializeField] private Image fromIcon, toIcon;
    [SerializeField] private TMP_Text fromText, toText;
    
    public override void Show()
    {
        if (Data.TryGetFromToData(out var data))
        {
            SetData(data.From, fromIcon, fromText);
            SetData(data.To, toIcon, toText);
            gameObject.SetActive(true);
        }
        else
        {
            Hide();
        }
        void SetData(SpriteIntPair stack, Image iconImage, TMP_Text amountText)
        {
            iconImage.sprite =  stack.Image;
            int amount = stack.Int;
            amountText.text = amount > 1 ? amount.ToString() : string.Empty;
        }
    }
    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
public abstract class DataUIModule<T> : MonoBehaviour
{
    protected T Data;
    public virtual void Init(T data)
    {
        this.Data = data;
    }
    public abstract void Show();
    public abstract void Hide();
}

public class FromToData
{
    public SpriteIntPair From, To;

    public FromToData(Sprite fromSprite, int fromInt, Sprite toSprite, int toInt)
    {
        From = new SpriteIntPair() { Image = fromSprite, Int = fromInt };
        To = new SpriteIntPair() { Image = toSprite, Int = toInt };
    }
}
public class SpriteIntPair
{
    public Sprite Image;
    public int Int;
}