using UnityEngine;

public class SpriteManager : MonoSingleton<SpriteManager>
{
    [System.Serializable]
    public class ItemSkinWorm
    {
        public Sprite head, headEat, body1, body2, body3, tail;// sprite cho player
    }
    public ItemSkinWorm[] itemSkinWorms;

    public Sprite spriteOn;

    public Sprite spriteOff;


    //[SerializeField] private List<ItemSkinWorm> itemSkinWorm = new List<ItemSkinWorm>();

    private static SpriteManager instance;

    private static GameObject parentObject;

    public static SpriteManager GetInstance()
    {
        if (!instance)
        {
            parentObject = GameObject.Find("SpriteManager");
            if (parentObject != null)
            {
                instance = parentObject.GetComponent<SpriteManager>();
            }
        }
        return instance;
    }
}
