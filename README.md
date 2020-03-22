# UnityQuestSystem
이 프로젝트는 Unity 프로젝트에서 SQLite를 사용해서 구현한 퀘스트 시스템 입니다.

## 시작하기

### 요구 사양

* Unity Editor (2019.3.3f1 or newer)
  
### 설치 방법

1. 기기에 Unity Editor (2019.3.3f1 or newer)를 설치합니다.
2. Git을 통해 UnityQueestSystem을 하드 드라이브에 클론합니다.  
~~~
git clone https://github.com/IronKim/UnityQuestSystem  
~~~
3. Unity 에디터로 Unity 프로젝트를 엽니다.
4. Unity 에디터를 통해 UnityQuestSystem 패키지를 가져옵니다.  
~~~
Assets -> Import Package -> Custom Package
~~~


## 시스템 구성

### 구성 요소
* Quest : 일종의 하나의 값 뜻 하며 기본 값으로 False를 가집니다. 이후 이 Quest가 해결된다면 True값으로 변경됩니다.
* Swtich : Quest를 이용하여 다단계의 상태를 만들 수 있으며 현재 해결된 Quest 값들로 하나의 상태 결과 값을 가집니다.
* SwitchController : 추상 클래스로 Switch의 변화를 구독하는 Observer 입니다. 
* QuestManager : Quest가 해결되는 정보를 발행하는 Publisher입니다. 


### Switch 계산 방식
Switch의 상태 중 계산 결과가 True인 마지막 상태의 상태 결과 값이 적용됩니다.
<details>
<summary>Ex) Switch [S0001]</summary>

Quest가 아무것도 클리어 되지 않은 상황

상태 | 계산식             | 계산 결과 | 상태 결과
---- | ------------------ | --------- | --------
기본 | -                  | True         | **True** (적용)
0    | [Q0001]            | False     | False
1    | [Q0002] & [Q0003]  | False     | True
<br>

Quest - [Q0001]가 해결 된 상태일때

상태 | 계산식             | 계산 결과 | 상태 결과
---- | ------------------ | --------- | --------
기본 | -                  | True      | True 
0    | [Q0001]            | True      | **False** (적용)
1    | [Q0002] & [Q0003]  | False     | True
<br>

Quest - [Q0001], [Q0002]가 클리어 된 상태일때

상태 | 계산식             | 계산 결과 | 상태 결과
---- | ------------------ | --------- | --------
기본 | -                  | True      | True 
0    | [Q0001]            | True      | **False** (적용)
1    | [Q0002] & [Q0003]  | False     | True
<br>

Quest - [Q0001], [Q0002], [Q0003]가 클리어 된 상태일때

상태 | 계산식             | 계산 결과 | 상태 결과
---- | ------------------ | --------- | --------
기본 | -                  | True      | True 
0    | [Q0001]            | True      | False 
1    | [Q0002] & [Q0003]  | True      | **True** (적용)
<br>

Quest - [Q0002], [Q0003]가 클리어 된 상태일때

상태 | 계산식             | 계산 결과 | 상태 결과
---- | ------------------ | --------- | --------
기본 | -                  | True      | True 
0    | [Q0001]            | False     | False 
1    | [Q0002] & [Q0003]  | True      | **True** (적용)
<br>

Quest - [Q0003]가 클리어 된 상태일때

상태 | 계산식             | 계산 결과 | 상태 결과
---- | ------------------ | --------- | --------
기본 | -                  | True      | **True** (적용)
0    | [Q0001]            | False     | False
1    | [Q0002] & [Q0003]  | False     | True
<br>

</details>





## 사용 방법
### 설정하기
1. Unity 에디터에서 UnityQuestSystem의 PreferencesWindow를 엽니다. 
~~~
QuestSystem -> PreferencesWindow
~~~
  
[![PreferencesWindow](https://ironkim.github.io/assets/image/project/unityquestsystem/preferences.png)](https://ironkim.github.io/assets/image/project/unityquestsystem/preferences.png ){: target="_blank"}  

1. 원하는 데이터베이스명을 입력하고 **결정** 버튼을 누르면 프로젝트 내로 StreamingAssets 디렉토리에 SQLite 데이터베이스가 생성됩니다.
~~~
Assets/StreamingAssets/DatabaseName
~~~

### Quest 생성하기
1. Unity 에디터에서 UnityQuestSystem의 EditQuestWindow 엽니다.
~~~
QuestSystem -> EditQuestWindow
~~~

[![QuestWindow](https://ironkim.github.io/assets/image/project/unityquestsystem/quest.png)](https://ironkim.github.io/assets/image/project/unityquestsystem/quest.png ){: target="_blank"}  

2. **QuestId**를 입력합니다. (필수)
3. **설명**을 입력합니다. (선택)
4. **생성** 버튼을 누릅니다.

### Switch 생성하기
1. Unity 에디터에서 UnityQuestSystem의 EditSwitchWindow 엽니다.
~~~
QuestSystem -> EditSwitchWindow
~~~

[![SwitchWindow](https://ironkim.github.io/assets/image/project/unityquestsystem/switch.png)](https://ironkim.github.io/assets/image/project/unityquestsystem/switch.png ){: target="_blank"}  

2. **QuestId**를 입력합니다. (필수)
3. **기본 결과**를 선택합니다.
4. **설명**을 입력합니다. (선택)
5. 필요에 따라 추가적인 **상태**를 생성합니다.
6. **생성** 버튼을 누릅니다.

### 조회하기
Unity 에디터에서 UnityQuestSystem의 SearchWindow를 통해서 생성된 Quest와 Switch를 조회할 수 있습니다.
~~~
QuestSystem -> SearchWindow
~~~

[![SearchQuestWindow](https://ironkim.github.io/assets/image/project/unityquestsystem/searchquest.png)](https://ironkim.github.io/assets/image/project/unityquestsystem/searchquest.png ){: target="_blank"}  
<br>

[![SearchSwitchWindow](https://ironkim.github.io/assets/image/project/unityquestsystem/searchswitch.png)](https://ironkim.github.io/assets/image/project/unityquestsystem/searchswitch.png ){: target="_blank"}  

### 게임 구현하기
1. 추상 클래스 **UnityQuestSystem.SwitchController**를 상속하여 Switch의 결과 값에 따른 행동을 구현합니다.
2. 구현한 SwitchController를 컴포넌트로 가진 GameObject를 Scene에 배치합니다.
3. **UnityQuestSystem.SQLiteManager**을 이용하여 생성한 SQLite 데이터베이스와 연결되도록 하는 GameObject를 Scene에 배치합니다.
~~~ c#
UnityQuestSystem.SQLiteManager.Instance.Connect("DatabaseName");
~~~
4. **UnityQuestSystem.QuestManager**를 이용하여 Quest를 해결하고 Switch를 업데이트하여 SwitchController를 작동시킵니다.
~~~ c#
// 퀘스트 해결
UnityQuestSystem.QuestManager.Instance.ClearQuest("Q0001");
UnityQuestSystem.QuestManager.Instance.ClearQuest("Q0002");
// 해결된 퀘스트에 따라 관련된 Switch를 업데이트
UnityQuestSystem.QuestManager.Instance.Update();
~~~

### Demo
Assets/Scene/QuestSystem/SampleScene

[![Demo](https://ironkim.github.io/assets/image/project/unityquestsystem/demo.jpg)](https://ironkim.github.io/assets/image/project/unityquestsystem/demo.jpg ){: target="_blank"}  


## Built with
* [Unity](https://unity.com/){: target="_blank"} 
* [SQLite4Unity3d](https://github.com/IronKim/SQLite4Unity3d){: target="_blank"} 
