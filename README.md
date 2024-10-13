# 【Unity】《二十一点》- Lab2 博客 

Video URL：https://www.bilibili.com/video/BV1gP2SY7E5n/

---

## 一、游戏设计  

1. 玩家  

    该游戏为单人游戏；在游戏中，玩家需进行收益与风险的权衡，根据手中的牌的情况，在“抽牌”与“停牌”两种行为中选择更合适的行为，最终赢得与对手的点数博弈； 

2. 目标  

    玩家的目标是使手中的牌的点数之和不超过二十一点且尽量大，以超过对手的点数之和； 

3. 操作  

    玩家点击“新的游戏”按钮后开始新的一局游戏； 

    开始游戏后，玩家可以点击“抽牌”按钮进行抽牌；点击“停牌”按钮停止抽牌，等待对手操作完毕； 

4. 规则  

    游戏开始后，玩家与对手分别抽取两张牌； 

    若玩家或对手手牌内唯二的两张牌的点数之和为21点（黑杰克），则玩家或对手直接赢得该局，该局结束；

    若该局未结束，则玩家与对手轮流进行抽牌，对手先手，每次一张；一方停牌后另一方可连续抽牌； 

    若玩家或对手手牌内的所有牌的点数之和大于21点（爆牌），则玩家或对手该局直接判负，该局结束； 

    若双方都选择停牌，则展示双方手牌，点数之和大的一方获得该局的胜利； 

5. 结果  

    若对手爆牌或双方停牌后玩家手牌的点数之和大，则玩家获得该局的胜利； 

    若玩家爆牌或双方停牌后对手手牌的点数之和大，则玩家该局判负；

---

## 二、游戏对象  

  该游戏的项目结构，游戏对象与场景效果如下图所示： 
  
  ![Image](./word/media/image1.png)

  1. Card对象为预制体，其子对象ValueText的文本内容默认为空；
  
  2. Card对象上挂载CardDisplay脚本，用于修改ValueText的文本内容； 
  
  ![Image](./word/media/image2.png)

  3. PlayerGrid对象中存在Grid Layout Group组件，用于展示Player的所有手牌；EnemyGrid对象同理； 
  
  ![Image](./word/media/image3.png)

  4. GameManager对象上挂载着Manager脚本，用于管理游戏的开始、进行与结束； 
  
  ![Image](./word/media/image4.png)

  5. Exercisers对象上挂载着Exercisers脚本，用于为抽取的卡牌添加运动效果；其中，CardArea中的Card对象为放置于Scene中的Card对象，CardPrefab的Card对象为预制体Card对象； 
  
  ![Image](./word/media/image5.png)

  6. WinShow，LoseShow与DrawShow对象为游戏胜负提示窗，初始不可见； 
  
  ![Image](./word/media/image6.png)

---

## 三、游戏实现 

1. CardDisplay

    ```cs
        public class CardDisplay : MonoBehaviour
        {
            public Text valueText;
        
            public void SetValueText(string value)
            {
                valueText.text = value;
            }
        }
    ```

2. Exercisers 

    变量direction用于记录运动方向，变量time用于记录当前已运动的时间，变量running用于记录当前是否应该运动；

    ```cs
        private string direction;
        private float time;
        private bool running = false;
        
        private GameObject newCard;
        public GameObject cardArea;
        public GameObject cardPrefab;
    ```

    调用Init()函数，重置上述变量值，并创建运动对象；

    ```cs
        public void Init(string dir)
        {
            newCard = GameObject.Instantiate(cardPrefab, cardArea.transform);
            time = 0;
            direction = dir;
            running = true;
        }
    ```

    Exercisers 类仅用于创建抽牌的动画效果，故newCard对象在运动完毕后销毁； 
  
    在本游戏中仅存在玩家抽牌，对手抽牌两种动画效果，故无需添加与传递运动时间，运动速度等参数； 

    ```cs
        void Update()
        {
            if (running)
            {
                if (time < 0.25)
                {
                    if (direction == "up")
                    {
                        newCard.transform.position += 2400 * Vector3.up * Time.deltaTime;
                    }
                    if (direction == "down")
                    {
                        newCard.transform.position -= 1600 * Vector3.up * Time.deltaTime;
                    }
                    time += Time.deltaTime;
                }
                else
                {
                    running = false;
                    Destroy(newCard);
                }
            }
        }
    ```

3. Manager

    利用变量signal在Manager脚本内实现类似互斥锁的机制；

    变量signal的值存在特定代码块执行完毕后自动修改，玩家与游戏进行交互调用函数修改两种修改方式；

    ```cs
        // Entities and their states / Model
        private List<string> heap = new List<string>();
        private string[] dir = new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        
        // Keep track of a player's or opponent's hand
        private List<string> enemy = new List<string>();
        private List<string> player = new List<string>();
        
        public GameObject cardPrefab;
        public GameObject enemyGrid;
        public GameObject playerGrid;
        public GameObject winShow;
        public GameObject loseShow;
        public GameObject drawShow;
        public GameObject startButton;
        public GameObject exerciser;
        
        // Judge if it's a player round or if the player will draw cards
        // -1 means that the player will no longer draw cards
        // 1 is the current player's turn, 0 is the opponent's turn
        private static int signal = 0;

        private bool gameRunning = false;
    ```

    调用Shuffle()函数，洗牌，返回打乱后的牌堆列表； 

    ```cs
        // Components /controls
        List<string> Shuffle(List<string> list)
        {
            System.Random random = new System.Random();
            List<string> newList = new List<string>();
            foreach(string item in list)
            {
                newList.Insert(random.Next(newList.Count + 1), item);
            }
            return newList;
        }
    ```

    调用Calculate()函数，计算并返回当前玩家或对手的所有手牌所能表示的最大的点数之和

    ```cs
       // Calculate the maximum number of points that can be represented by all hands of a side
       int Calculate(List<string> list)
       {
           int ans = 0, A_num = 0;
           foreach(string item in list)
           {
               switch(item) 
               { 
                   case "A": A_num++; break;
                   case "K": ans += 10; break;
                   case "Q": ans += 10; break;
                   case "J": ans += 10; break;
                   default:ans += int.Parse(item); break;
               }
           }
           if (A_num > 0)
           {
               ans += A_num;
               if (ans + 10 <= 21)
               {
                   ans += 10;
               }
           }
           return ans;
       }
    ```

    调用DrawCard()函数，创建抽牌的动画效果，并调用DrawToEnemy()或DrawToPlayer()函数；该函数并不修改Model的参数值；
   
    调用DrawToEnemy()与DrawToPlayer()函数，创建Card对象并将其在游戏中显示出来，同时修改Model的参数值； 

    ```cs
           void DrawToEnemy()
        {
            GameObject newCard = GameObject.Instantiate(cardPrefab, enemyGrid.transform);
            enemy.Add(heap[0]);
            newCard.GetComponent<CardDisplay>().SetValueText(heap[0]);
            heap.RemoveAt(0);
        }
        
        void DrawToPlayer()
        {
            GameObject newCard = GameObject.Instantiate(cardPrefab, playerGrid.transform);
            player.Add(heap[0]);
            newCard.GetComponent<CardDisplay>().SetValueText(heap[0]);
            heap.RemoveAt(0);
        }
        
        void DrawCard(bool flag)
        {
            if (!flag)
            {
                exerciser.GetComponent<Exercisers>().Init("up");
                DrawToEnemy();
            }
            else
            {
                exerciser.GetComponent<Exercisers>().Init("down");
                DrawToPlayer();
            }
        }
    ```

    在游戏运行时，玩家按下“新的游戏”，“抽牌”或“停牌”按钮后，分别调用OnClickStart()，OnClickContinue()或OnClickEnd()函数；

    ```cs
        // Button interactions
        public void OnClickContinue()
        {
            if(gameRunning)
            {
                signal = 1;
            }
        }
        public void OnClickEnd()
        {
            if (gameRunning)
            {
                signal = -1;
            }
        }
        public void OnClickStart()
        {
            GameInit();
            GameRunning();
        }
    ```

    新的游戏开始时，调用GameInit()函数，重置Model的参数值并恢复游戏UI；

    上述操作完毕后，向上移动enemyGrid以隐藏对手的手牌，并为玩家与对手二人各抽取两张牌；

    ```cs
        void GameInit()
        {
            // Refresh model
            signal = 0;
            heap.Clear();
            enemy.Clear();
            player.Clear();
            gameRunning = false;
        
            // Set the UI controls to invisible
            winShow.SetActive(false);
            loseShow.SetActive(false);
            drawShow.SetActive(false);
            startButton.SetActive(false);
        
            // Clear Grid
            foreach (Transform child in enemyGrid.GetComponent<Transform>())
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in playerGrid.GetComponent<Transform>())
            {
                Destroy(child.gameObject);
            }
        
            // Initialize the deck and shuffle
            for (int i = 0; i < 13; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    heap.Add(dir[i]);
                }
            }
            heap = Shuffle(heap);
        
            // Hide the opponent's hand
            enemyGrid.transform.position = enemyGrid.transform.position + new Vector3(0, 200, 0);
        
            // Initial dealing
            DrawCard(false); DrawCard(false); DrawCard(true); DrawCard(true);
        }
    ```
    
    调用GameInit()函数后，调用GameRunning()函数；

    若玩家或对手的手牌满足黑杰克，则游戏胜负已定，无需继续进行；

    若不满足，则设置gameRunning的值为true，游戏继续进行，后续回合在Update()函数内实现；

    ```cs
        void GameRunning()
        {
            // Judge Black Jack 
            int playerSum = Calculate(player);
            int enemySum = Calculate(enemy);
            if (playerSum == 21 || enemySum == 21)
            {
                if (playerSum == enemySum)
                {
                    GameEnd(0);
                }
                else
                if (playerSum == 21)
                {
                    GameEnd(1);
                }
                else
                if(enemySum == 21)
                {
                    GameEnd(-1);
                }
                return;
            }
        
            gameRunning = true;
        }
    ```
    
    胜负确定后，调用GameEnd()函数，终止游戏回合进行，并根据胜负情况显示对应的提示信息；

    上述操作完毕后，向下移动enemyGrid以显示对手的手牌，并重新设置“新的开始”按钮为可见；

    ```cs
        void GameEnd(int flag)
        {
            gameRunning = false;
        
            // Set the correct end window to visible
            switch (flag)
            {
                case 1:winShow.SetActive(true);break ;
                case -1:loseShow.SetActive(true);break ;
                case 0:drawShow.SetActive(true) ;break ;
            }
        
            // Show the opponent's hand
            enemyGrid.transform.position = enemyGrid.transform.position + new Vector3(0, -200, 0);
        
            // Prepare for the next game
            startButton.SetActive(true);
        } 
    ```

    若gameRunning的值为true，则进入游戏回合的判断处理；

    玩家与对手每次抽牌后，都将进行是否爆牌的判断；若一方爆牌，则另一方立即获得游戏胜利；
    
    ```cs
        void Update()
        {
            if (gameRunning)
            {
                if (signal == 1)
                {
                    signal = 0;
                    DrawCard(true);
                    int sum = Calculate(player);
                    // Judge if the player bust
                    if (sum > 21)
                    {
                        GameEnd(-1);
                    }
                }
                else
                if (signal == 0 || signal == -1) 
                {
                    int sum = Calculate(enemy);
                    // Judge if the opponent should draw card
                    if (sum < 15)
                    {
                        DrawCard(false);
                        sum = Calculate(enemy);
                    }
                    // Judge if the opponent bust
                    if (sum > 21)
                    {
                        GameEnd(1);
                    }
                    if (sum >= 15 && sum <= 21 && signal == -1)
                    {
                        int score = Calculate(player);
                        if (score == sum)
                        {
                            GameEnd(0);
                        }
                        else
                        if (score > sum)
                        {
                            GameEnd(1);
                        }
                        else
                        if (score < sum)
                        {
                            GameEnd(-1);
                        }
                    }
                    // If the player will draw card, wait for it's operation
                    if (signal == 0)
                    {
                        signal = 10;
                    }
                }
            }
        }
    ```
