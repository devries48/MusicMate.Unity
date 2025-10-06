using UnityEngine;

public class DataTabAnimator : MusicMateBehavior
{
    [Header("Tabs")] 
    [SerializeField] TabItemAnimator tagsTab;
    [SerializeField] TabItemAnimator infoTab;
    [SerializeField] TabItemAnimator linksTab;
    [SerializeField] TabItemAnimator urlsTab;

    [Header("Controllers")] 
    [SerializeField] ShowTabContentController tagsDataController;
    [SerializeField]ShowTabContentController infoDataController;
    [SerializeField] ShowTabContentController linksDataController;
    [SerializeField] ShowTabContentController urlsDataController;
}