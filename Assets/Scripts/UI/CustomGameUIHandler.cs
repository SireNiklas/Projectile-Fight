using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Steamworks.Data;
using Steamworks;
using QFSW.QC;
using System.Linq;

public class CustomGameUIHandler : MonoBehaviour
{

    GameObject UIObjectToHide, UIObjecToShow;
    private static VisualTreeAsset listItem;

    public SteamId lobbyId;

    //public Lobby[] lobbiesList;

    //private List<Tuple<string, SteamId>> items;
    
    private async void OnEnable()
    {
        SteamManager.Instance.activeLobbies.Clear();
        SteamManager.Instance.lobbiesList = await SteamMatchmaking.LobbyList.WithKeyValue("isTesting", "TRUE").RequestAsync();

        //lobbiesList = await SteamMatchmaking.LobbyList.RequestAsync();

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        VisualElement body = root.Q<VisualElement>("bodycontainer");

        Button createGameBtn = root.Q<Button>("creategame");
        Button backBtn = root.Q<Button>("back");

        backBtn.clicked += () => UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[1], UIObjecToShow = UIManager.Instance.UIObjects[0]);

        createGameBtn.clicked += () => { UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[1], UIObjecToShow = UIManager.Instance.UIObjects[2]); /*SteamManager.Instance.StartHost(4); this.gameObject.SetActive(false);*/ };

        //if (lobbiesList != null)
        //{
        //    foreach (Lobby b in lobbiesList.ToList())
        //    {
        //        activeLobbies.Add(b);
        //        Debug.Log(activeLobbies);
        //        Debug.Log(activeLobbies.Count);
        //    }
        //}

        if (SteamManager.Instance.lobbiesList != null)
        {

            //Create a list of data.In this case, numbers from 1 to 1000.
            foreach (Lobby lobby in SteamManager.Instance.lobbiesList.ToList())
            {

                SteamManager.Instance.activeLobbies.Add(lobby);
                Debug.Log(SteamManager.Instance.activeLobbies.Count);

            }

            int i = 0;
            
            int itemCount = SteamManager.Instance.activeLobbies.Count;
            var items = new List<Tuple<string, SteamId, int, int>>(itemCount);
            for (i = 0; i < itemCount; i++)
            {
                items.Add(Tuple.Create(SteamManager.Instance.activeLobbies[i].GetData("lobbyName"), SteamManager.Instance.activeLobbies[i].Id, SteamManager.Instance.activeLobbies[i].MemberCount, SteamManager.Instance.activeLobbies[i].MaxMembers));
                Debug.Log(items[i].Item1);
                //Debug.Log(activeLobbies[i]);
            }

            // The "makeItem" function is called when the
            // ListView needs more items to render.
            Func<VisualElement> makeItem = () =>
            {
                var root = new VisualElement();
                var label = new Label();
                root.Add(label);
                return root;
            };

            // As the user scrolls through the list, the ListView object
            // recycles elements created by the "makeItem" function,
            // and invoke the "bindItem" callback to associate
            // the element with the matching data item (specified as an index in the list).
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                var label = e.Q<Label>();
                label.text = items[i].Item1 + " | Members: " + items[i].Item3 + "/" + items[i].Item4;
            };

            // Provide the list view with an explict height for every row
            // so it can calculate how many items to actually display
            const int itemHeight = 64;

            var listView = new ListView(items, itemHeight, makeItem, bindItem);

            listView.onItemsChosen += objects => SteamManager.Instance.JoinLobby(items[listView.selectedIndex].Item2);
            //SteamManager.Instance.currLobby = items[listView.selectedIndex].Item2;
            //listView.onSelectionChange += objects => Debug.Log(items[listView.selectedIndex].ToString());

            // List Style
            listView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
            listView.selectionType = SelectionType.Single;
            listView.style.flexGrow = 1.0f;
            listView.style.fontSize = 48f;
            listView.style.justifyContent = Justify.Center;
            listView.showBoundCollectionSize = true;

            body.Add(listView);
        }
    }

}
//
// [Serializable]
// public class MultiTypeList<a, b>
// {
//     private MultiTypeList<a, b> getLobbiesList = new MultiTypeList<a, b>();
// }