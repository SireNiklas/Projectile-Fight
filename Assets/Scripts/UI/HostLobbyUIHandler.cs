using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Steamworks.ServerList;
using UnityEngine;
using UnityEngine.UIElements;

public class HostLobbyUIHandler : MonoBehaviour
{
    GameObject UIObjectToHide, UIObjecToShow;


    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement body = root.Q<VisualElement>("bodycontainer");

        Button startGameBtn = root.Q<Button>("startgame");
        Button backBtn = root.Q<Button>("back");
        Label lobbyNameLbl = root.Q<Label>("lobbyNameLbl");

        lobbyNameLbl.text = UIManager.Instance.lobbyName;
        startGameBtn.clicked += () => Debug.Log("START GAME!!");
        backBtn.clicked += () => UIManager.Instance.SwapUI(UIObjectToHide = UIManager.Instance.UIObjects[3], UIObjecToShow = UIManager.Instance.UIObjects[1]);

        int i = 0;
        
         // foreach (var member in )
         // {
         //     Debug.Log($"FRIENDS: {member.Name}");
         // }

         for (i = 0; i < SteamManager.Instance.currLobby.MemberCount; i++)
         {
             foreach (var member in SteamManager.Instance.currLobby.Members)
             {
                 SteamManager.Instance.currLobby.SetMemberData("memberName", SteamClient.Name);
                 Debug.Log(SteamManager.Instance.currLobby.GetMemberData(member, "memberName"));
             }
         }

         
        int itemCount = 0;
        //var items = new List<Tuple<SteamId, SteamId, int, int>>(itemCount);
        var items = new List<string>(itemCount);
        for (i = 0; i < itemCount; i++)
        {
            //items.Add(Tuple.Create(SteamManager.Instance.activeLobbies[i].GetData("lobbyName"), SteamManager.Instance.activeLobbies[i].Id, SteamManager.Instance.activeLobbies[i].MemberCount, SteamManager.Instance.activeLobbies[i].MaxMembers));
            items.Add(SteamClient.Name);
            Debug.Log(SteamClient.Name);
            //Debug.Log(items[i].Item1);
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
            label.text = SteamClient.Name;
        };

        // Provide the list view with an explict height for every row
        // so it can calculate how many items to actually display
        const int itemHeight = 64;

        var listView = new ListView(items, itemHeight, makeItem, bindItem);

        //listView.onItemsChosen += objects => SteamManager.Instance.JoinLobby(items[listView.selectedIndex].Item2);
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
