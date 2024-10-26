using System;
using UnityEngine;

[SerializeField]
public class Deal
{
    public TokenStack Pay;
    public Card Goods;

    public Deal(CardID goodsID, TokenID payTokenID, int payAmount)
    {
        Pay = new TokenStack { Token = payTokenID.ToToken(), Amount = payAmount };
        Goods = goodsID.ToCard();
    }

    internal void Execute()
    {
        TokenHandler.Instance.RemoveToken(Pay.Token.ID, Pay.Amount);
        CardHandler.Instance.AddCardAnimated(Goods.ID, UnityEngine.Object.FindObjectOfType<Player>().Position);
    }
}

