using System;
using System.Collections;
using System.Collections.Generic;
using QuestSystem;
using UnityEngine;

#if QUEST_SYSTEM_TEST
using System.Linq;
#endif

public class TestManager : MonoBehaviour
{
    [SerializeField] 
    private string _dbName = string.Empty;
    
    //
    // SQLite 연결
    //
    private void Awake()
    {
        SQLiteManager.Instance.Connect(_dbName);
        
    }
    
    //
    // 매 프래임마다 클리어된 퀘스트들이 있다면 관련된 Switch들을 업데이트 해준다.
    // 이런 방식으로 구현하면 클리어한다면 SwitchController가 다음 프래임부터 변화된 값을 사용한다는 점에서 주의
    // 해당 프래임에서 변화된 값을 사용을 원할 시 QuestManager.Instance.QuestClear()이후 QuestManager.Instance.Update()를 직접 실행
    //
    private void LateUpdate()
    {
        QuestManager.Instance.Update();
    }




    #region Test

    [SerializeField] 
    private bool _isShowingTestUi = false;

    private const int RowHeight = 30;
    private const int RowWidth = 160;
    private const int ScrollViewHeight = 210;
    
    private GUIStyle customStyle = null;
    
    
    private Vector2 _questScrollViewVector = Vector2.zero;
    private List<QuestData> _questDataList = null;
    private List<QuestData> _searchQuestDataList = null;
    private string _searchQuestText = string.Empty;

    
    private Vector2 _switchScrollViewVector = Vector2.zero;
    private List<SwitchData> _searchSwitchDataList = null;
    private string _searchSwitchText = string.Empty;
    
    
    
    private void OnGUI()
    {
        if(!_isShowingTestUi)
        {
            return;
        }
        
        if (null == customStyle)
        {
            customStyle = new GUIStyle();
            customStyle.alignment = TextAnchor.MiddleLeft;
            customStyle.normal.textColor = Color.white;
        }

        if (null == _questDataList)
        {
            _questDataList = SQLiteManager.Instance.GetAllQuestDataList();
            _searchQuestDataList = _questDataList;
        }

        DrawQuestWindow(new Rect(10, 10, 200, 275));
        DrawSwitchWindow(new Rect(10, 295, 200, 275));

    }


    void DrawQuestWindow(Rect drawRect)
    {
        //박스 생성
        GUI.Box(drawRect, "Quest Menu");

        //검색창 생성
        DrawSearchBar(drawRect, ref _questDataList, ref _searchQuestDataList, ref _searchQuestText, ref _switchScrollViewVector,
            (questData, searchText) => questData.QuestId.Contains(searchText));
        
        //리스트창 생성
        DrawTable(drawRect, ref _searchQuestDataList, ref _questScrollViewVector, (questData, idx) =>
        {
            GUI.Label(new Rect(0, idx * RowHeight, RowWidth, RowHeight), questData.QuestId, customStyle);
            if (!QuestManager.Instance.IsQuestClear(questData.QuestId))
            {
                if(GUI.Button(new Rect(110, idx * RowHeight + 2, 50, RowHeight - 4), "Clear"))
                {
                    QuestManager.Instance.ClearQuest(questData.QuestId);
                }
            }
        });
    }

    
    void DrawSwitchWindow(Rect drawRect)
    {
        //박스 생성
        GUI.Box(drawRect, "Switch Menu");

        var switchDataList = QuestManager.Instance.SwitchDataDic.Select(pair => pair.Value).ToList();
        _searchSwitchDataList = switchDataList;
        
        //검색창 생성
        DrawSearchBar(drawRect, ref switchDataList, ref _searchSwitchDataList, ref _searchSwitchText, ref _switchScrollViewVector,
            (switchData, searchText) => switchData.SwitchId.Contains(searchText));

        //리스트창 생성
        DrawTable(drawRect, ref _searchSwitchDataList, ref _switchScrollViewVector, (switchData, idx) =>
        {
            GUI.Label(new Rect(0, idx * RowHeight, RowWidth, RowHeight), switchData.SwitchId, customStyle);

            bool switchResult = switchData.CurrentResult;
            customStyle.normal.textColor = switchResult ? new Color32(117, 179, 255, 255) : new Color32(255, 177, 177, 255);
            GUI.Label(new Rect(120, idx * RowHeight, RowWidth, RowHeight), switchResult.ToString(), customStyle);
            customStyle.normal.textColor = Color.white;
        });
    }


    void DrawSearchBar<T>(Rect drawRect, ref List<T> dataList, ref List<T> searchDataList, ref string searchText, ref Vector2 scrollViewVector, Func<T, string, bool> searchAction)
    {
        string beforeSearchText = searchText;
        searchText = GUI.TextField(new Rect(drawRect.x + 10, drawRect.y + 25, 180, 20), searchText);
        if (searchText != beforeSearchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                searchDataList = dataList;
            }
            else
            {
                string tempSearchText = searchText;
                searchDataList = dataList.Where(questData => searchAction(questData, tempSearchText)).ToList();
            }
            scrollViewVector = Vector2.zero;
        }
    }

    void DrawTable<T>(Rect drawRect, ref List<T> searchDataList, ref Vector2 scrollViewVector, Action<T, int> drawRowAction)
    {
        int scrollHeight = searchDataList.Count * RowHeight;
        scrollViewVector = GUI.BeginScrollView (new Rect (drawRect.x + 10, drawRect.y + 55, 180, ScrollViewHeight), scrollViewVector, new Rect (0, 0, 160, scrollHeight));

        //모든 Row를 그리려면 오버해드가 너무 크다
        //ScrollPosition에 맞춰서 해당 위치만 그린다.
        int middleIdx = (int)(searchDataList.Count * (scrollViewVector.y / (scrollHeight - ScrollViewHeight)));
        int rowCount = ScrollViewHeight / RowHeight;
        for (int idx = Mathf.Max(0, middleIdx - rowCount); 
            idx <= middleIdx + rowCount && idx < searchDataList.Count; 
            ++idx)
        {
            var questData = searchDataList[idx];
            drawRowAction(searchDataList[idx], idx);
        }
        
        GUI.EndScrollView();
    }

    #endregion
    
}
