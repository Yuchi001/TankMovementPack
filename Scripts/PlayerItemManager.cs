using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemManager : MonoBehaviour
{
    [SerializeField] private PartItem defaultTop;
    [SerializeField] private PartItem defaultBottom;
    [SerializeField] private PartItem defaultTracks;
    [SerializeField] private GameObject itemPreset;
    private PlayerScriptsManager playerSM => GetComponent<PlayerScriptsManager>();
    private GameObject PartTop => transform.GetChild(0).gameObject;
    private GameObject PartBottom => transform.GetChild(1).gameObject;
    private GameObject ExhaustPipe => transform.GetChild(transform.childCount - 2).gameObject;
    private GameObject Tracks => transform.GetChild(2).gameObject;
    public BottomPart BottomMain
    {
        get { return _bottomMain; }
        private set
        {
            SetBottomStats(value);
            _bottomMain = value;
        }
    }
    private BottomPart _bottomMain;
    public TopPart TopMain
    {
        get { return _topMain; }
        private set
        {
            SetTopStats(value);
            _topMain = value;
        }
    }
    private TopPart _topMain;

    public TrackPart TracksMain
    {
        get { return _tracksMain; }
        private set
        {
            SetTracksStats(value);
            _tracksMain = value;
        }
    }
    private TrackPart _tracksMain;

    private Item upgrade_bottom_1;
    private Item upgrade_bottom_2;

    private Item upgrade_top_1;
    private Item upgrade_top_2;

    private void Setup()
    {
        EquipPartItem(defaultBottom);
        EquipPartItem(defaultTop);
        EquipPartItem(defaultTracks);
    }
    void Start()
    {
        Setup();
    }
    void Update()
    {

    }
    public void PickUpItem(Item item)
    {
        GameManager.Instance.inventory.AddItem(item);

        switch (item.itemType)
        {
            case EItemType.equip:
                //PickUpEquipItem((EquipItem)item);
                break;
            case EItemType.part:
                //PickUpPartItem((PartItem)item);
                EquipPartItem((PartItem)item);
                break;
        }
    }
    private void EquipPartItem(PartItem item)
    {
        var itemType = (PartItem)item;
        switch (itemType.ReturnItemType())
        {
            case "BottomPart":
                BottomMain = (BottomPart)item;
                break;
            case "TopPart":
                TopMain = (TopPart)item;
                break;
            case "TrackPart":
                TracksMain = (TrackPart)item;
                break;
        }
    }
    private void DropPartItem(PartItem item)
    {
        var itemType = (PartItem)item;
        switch (itemType.ReturnItemType())
        {
            case "BottomPart":
                BottomMain = (BottomPart)defaultBottom;
                break;
            case "TopPart":
                TopMain = (TopPart)defaultTop;
                break;
            case "TrackPart":
                TracksMain = (TrackPart)defaultTracks;
                break;
        }
        SpawnItem(item);
    }

    private void SetBottomStats(BottomPart value)
    {
        float armor = playerSM.GetStat(EStatName.armor) + value.armor - (_bottomMain == null ?  0 : _bottomMain.armor);
        float health = playerSM.GetStat(EStatName.health) + value.health - (_bottomMain == null ? 0 : _bottomMain.health);
        float weight = playerSM.GetStat(EStatName.weight) + value.weight- (_bottomMain == null ? 0 : _bottomMain.weight);
        float reflectionBulletChance = playerSM.GetStat(EStatName.reflectionBulletChance) + value.reflectionBulletChance - (_bottomMain == null ? 0 : _bottomMain.reflectionBulletChance);
        playerSM.SetStat(EStatName.armor, armor);
        playerSM.SetStat(EStatName.health, health);
        playerSM.SetStat(EStatName.weight, weight);
        playerSM.SetStat(EStatName.reflectionBulletChance, reflectionBulletChance);

        PartBottom.GetComponent<SpriteRenderer>().sprite = value.partSprite;
        Tracks.transform.GetChild(0).localPosition = value.trackPosition;
        Tracks.transform.GetChild(1).localPosition = new Vector2(-value.trackPosition.x, value.trackPosition.y);
        ExhaustPipe.transform.localPosition = value.exhaustPipePosition;
        PartTop.transform.localPosition = value.turretPosition;

        SpawnItemWithCheck(_bottomMain, value);
    }
    private void SetTopStats(TopPart value)
    {
        float armor = playerSM.GetStat(EStatName.armor) + value.armor - (_topMain == null ? 0 : _topMain.armor);
        float range = playerSM.GetStat(EStatName.range) + value.range - (_topMain == null ? 0 : _topMain.range);
        float attackSpeed = playerSM.GetStat(EStatName.attackSpeed) + value.attackSpeed - (_topMain == null ? 0 : _topMain.attackSpeed);
        float rotateSpeed = playerSM.GetStat(EStatName.rotateSpeed) + value.rotateSpeed - (_topMain == null ? 0 : _topMain.rotateSpeed);
        playerSM.SetStat(EStatName.armor, armor);
        playerSM.SetStat(EStatName.attackSpeed, attackSpeed);
        playerSM.SetStat(EStatName.rotateSpeed, rotateSpeed);
        playerSM.SetStat(EStatName.range, range);

        PartTop.GetComponent<SpriteRenderer>().sprite = value.partSprite;
        PartTop.transform.GetChild(0).localPosition = value.barrelPosition;
        PartTop.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = value.barrelSprite;
        PartTop.transform.GetChild(3).localPosition = value.shootPos;

        SpawnItemWithCheck(_topMain, value);
    }
    private void SetTracksStats(TrackPart value)
    {
        float movementSpeed = playerSM.GetStat(EStatName.movementSpeed) + value.movementSpeed - (_tracksMain == null ? 0 : _tracksMain.movementSpeed);
        float acceleration = playerSM.GetStat(EStatName.acceleration) + value.accelerate - (_tracksMain == null ? 0 : _tracksMain.accelerate);
        float turningSpeed = playerSM.GetStat(EStatName.turiningSpeed) + value.turningSpeed - (_tracksMain == null ? 0 : _tracksMain.turningSpeed);
        float armor = playerSM.GetStat(EStatName.armor) + value.armor - (TracksMain == null ? 0 : _tracksMain.armor);
        playerSM.SetStat(EStatName.turiningSpeed, turningSpeed);
        playerSM.SetStat(EStatName.movementSpeed, movementSpeed);
        playerSM.SetStat(EStatName.acceleration, acceleration);
        playerSM.SetStat(EStatName.armor, armor);

        foreach(SpriteRenderer sr in Tracks.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.sprite = value.partSprite;
        }

        SpawnItemWithCheck(_tracksMain, value);

    }
    private void SpawnItemWithCheck<T>(T part, T item)
    {
        if(part != null && !(part as PartItem).isDefault)
        {
            SpawnItem(part as Item);
        }
    }
    private void EquipEquipItem(EquipItem item)
    {

    }
    private void PickUpPartItem(PartItem item)
    {

    }
    private void PickUpEquipItem(EquipItem item)
    {

    }
    private void SpawnItem(Item item)
    {
        GameManager.Instance.inventory.DropItem(item);

        ItemObject io = Instantiate(itemPreset, transform.position, Quaternion.identity).GetComponent<ItemObject>();
        io.item = item;
    }
}
