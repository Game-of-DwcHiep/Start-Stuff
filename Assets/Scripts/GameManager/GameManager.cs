using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DwcHyep;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>, ISaveable
{
    public int rows;
    public int cols;

    public float timeDropHand = 0.5f;

    public Camera _camera;

    public GameObject _BG;

    public GameObject wormController;

    [SerializeField] private string saveFileName = null;

    public GameObject cellPrefabWoodModel;

    public GameObject cellPrefabWoodNormal;
    public GameObject cellPrefabWoodScrew;
    public GameObject cellPrefabWoodHint;// box nay bắt buộc phải ăn thì mới win được
    public GameObject mapGroundTop;
    public GameObject mapGroundBot;
    public Transform gridData1;
    public Transform gridData2;

    public int[,] grid;
    public GameObject[,] cellObjects;
    public List<BoxController> boxes;

    public bool isMoving;// để check trạng thái player không thể di chuyển

    private bool createdBox = false;
    // Thêm biến TextAsset để gắn file JSON từ Editor
    public TextAsset[] listDataLevel;
    public int levelSelect;// level user chon choi hien tai

    // data level
    [SerializeField] private int coinsToStart = 0;
    public int LevelNumber { get; set; }// level cao nhat cua user
    public string SaveFilePath { get { return Application.persistentDataPath + string.Format("/{0}.json", saveFileName); } }

    public string SaveId { get { return saveFileName; } }
    public int Coins { get; private set; }

    public DropHand dropHandBox1;
    public DropHand dropHandBox2;

    public bool checkCreateMap = false;// biến này để kiểm tra đang chạy coroutine tạo map thì sẽ k làm mới map hay thoát ra được

    //[SerializeField] private int coinsMultiplier = 10;

    // xu ly data game
    public override void Awake()
    {
        Application.targetFrameRate = 60;

        base.Awake();

        SaveManager.Instance.Register(this);

        if (!LoadSave())
        {
            // If no save file exists then set the starting values
            Coins = coinsToStart;
            LevelNumber = 1;           
        }
        ResizeBackgroundToFitCamera();
    }
    // play game
    public void PlayGameInLevel(int levelData, bool useHint)
    {
        createdBox = true;
        if(checkCreateMap)// nếu đang chưa chạy xong tao map thì không được tạo map mới
        {
            return;
        }
        if (listDataLevel[levelData] != null)
        {
            if (!useHint)
            {
                LoadGridFromFile(listDataLevel[levelData].text);
            }
            else
            {
                LoadGridFromFileUseHint(listDataLevel[levelData].text);
            }    
            levelSelect = levelData;
        }
        else
        {
            Debug.LogWarning("Chưa gắn file JSON vào Inspector!");
        }     

    }
     
    public void ClearBox()
    {
        // Xóa tất cả các box hiện tại
        foreach (BoxController box in boxes)
        {
            if (box != null && box.gameObject != null)
            {
                Destroy(box.gameObject);
            }
        }

        // Xóa các đối tượng con trong gridData1, ngoại trừ dropHandBox1
        foreach (Transform child in gridData1)
        {
            if (child.gameObject != dropHandBox1.targetHand)
            {
                Destroy(child.gameObject);
            }
        }

        // Xóa các bản đồ nếu tồn tại
        GameObject maptop = GameObject.Find("map_top");
        GameObject mapbot = GameObject.Find("map_bot");
        if (maptop != null)
        {
            Destroy(maptop);
        }
        if (mapbot != null)
        {
            Destroy(mapbot);
        }

        // Xóa sâu (nếu cần)
        ClearWorm();
    }

    private void ClearWorm()
    {
        // Tìm sâu trong scene
        GameObject existingWorm = GameObject.Find("WormController");

        // Nếu sâu tồn tại, xóa nó
        if (existingWorm != null)
        {
            Destroy(existingWorm);
        }
    }

    // doc file lấy data của 2 mảng trong file, chơi bình thường
    public void LoadGridFromFile(string jsonData)
    {
        // Deserialize JSON data
        BinaryJsonWrapper loadedData = JsonUtility.FromJson<BinaryJsonWrapper>(jsonData);

        if (loadedData == null || loadedData.data1 == null || loadedData.data2 == null)
        {
            Debug.LogError("Không thể tải dữ liệu từ file JSON.");
            return;
        }

        int newRows = loadedData.data1.Length;
        int newCols = loadedData.data1[0].Length;

        rows = loadedData.data2.Length;
        cols = loadedData.data2[0].Length;

        if (!IsValidData(newRows, newCols))
        {
            Debug.LogError("Dữ liệu JSON không hợp lệ!");
            return;
        }
        ClearBox();
        checkCreateMap = true;
        UpdateCameraPosition();
        CreateMapObjects();
        StartCoroutine(CreateBoxesFromData1(loadedData, newRows, newCols));
        StartCoroutine(CreateMap2(loadedData));
    }

    public void LoadGridFromFileUseHint(string jsonData)
    {
        // Deserialize JSON data
        BinaryJsonWrapper loadedData = JsonUtility.FromJson<BinaryJsonWrapper>(jsonData);

        if (loadedData == null || loadedData.data1 == null || loadedData.data3 == null)
        {
            Debug.LogError("Không thể tải dữ liệu từ file JSON.");
            return;
        }

        int newRows = loadedData.data1.Length;
        int newCols = loadedData.data1[0].Length;

        rows = loadedData.data3.Length;
        cols = loadedData.data3[0].Length;

        if (!IsValidData(newRows, newCols))
        {
            Debug.LogError("Dữ liệu JSON không hợp lệ!");
            return;
        }
        ClearBox();
        checkCreateMap = true;
        UpdateCameraPosition();
        CreateMapObjects();
        StartCoroutine(CreateBoxesFromData1(loadedData, newRows, newCols));
        StartCoroutine(CreateMap2UseHint(loadedData));
    }
    private IEnumerator CreateMap2(BinaryJsonWrapper loadedData)
    {
        yield return new WaitForSeconds(2f);
        RecreateGridFromData2(loadedData);
        StartCoroutine(CreateWorm());
    }
    private IEnumerator CreateMap2UseHint(BinaryJsonWrapper loadedData)
    {
        yield return new WaitForSeconds(2f);
        RecreateGridFromData3(loadedData);
        StartCoroutine(CreateWorm());
    }

    private bool IsValidData(int newRows, int newCols)
    {
        return newRows > 0 && newCols > 0;
    }

    private void UpdateCameraPosition()
    {
        switch(rows)
        {
            case 2:
                _camera.orthographicSize = 14f;
                break;
            case 3:
                _camera.orthographicSize = 16f;
                break;
            case 4:
                _camera.orthographicSize = 18f;
                break;
            case 5:
                _camera.orthographicSize = 20f;
                break;
            case 6:
                _camera.orthographicSize = 22f;
                break;
            case 7:
                _camera.orthographicSize = 24f;
                break;
            case 8:
                _camera.orthographicSize = 26f;
                break;
            case 9:
                _camera.orthographicSize = 28f;
                break;
            case 10:
                _camera.orthographicSize = 30f;
                break;
            default:
                _camera.orthographicSize = 30f;
                break;
        }    
        _camera.transform.position = new Vector3(0, 2 - rows, -10);
        _BG.transform.position = new Vector3(0, 2 - rows, 3);

        // Điều chỉnh kích thước của _BG để vừa với màn hình
        ResizeBackgroundToFitCamera();
    }

    public bool IsAnyBoxStillExists()
    {
        // Kiểm tra xem danh sách còn object nào không null
        return boxes.Any(box => box != null);
    }

    private void ResizeBackgroundToFitCamera()
    {
        float cameraHeight = _camera.orthographicSize * 2;
        float cameraWidth = cameraHeight * _camera.aspect;

        // Lấy kích thước gốc của background sprite
        SpriteRenderer bgSpriteRenderer = _BG.GetComponent<SpriteRenderer>();

        if (bgSpriteRenderer != null)
        {
            Vector2 bgSize = bgSpriteRenderer.sprite.bounds.size;

            // Tính toán scale để vừa với kích thước camera
            float scaleX = cameraWidth / bgSize.x;
            float scaleY = cameraHeight / bgSize.y;

            // Cập nhật localScale cho _BG
            _BG.transform.localScale = new Vector3(scaleX, scaleY, 1);
        }
    }
    private void CreateMapObjects()
    {
        Vector2 posMap1 = new Vector2(0, 6);
        Vector2 posMap2 = new Vector2(0, -rows);

        GameObject maptop = Instantiate(mapGroundTop, posMap1, Quaternion.identity);
        maptop.name = "map_top";

        GameObject mapbot = Instantiate(mapGroundBot, posMap2, Quaternion.identity);
        mapbot.name = "map_bot";
    }

    private IEnumerator CreateBoxesFromData1(BinaryJsonWrapper loadedData, int newRows, int newCols)
    {
        yield return new WaitForSeconds(1f);
        Vector2 offset1 = new Vector2(0, rows + 6);
        gridData1.position = offset1;

        for (int i = 0; i < newRows; i++)
        {
            for (int j = 0; j < newCols; j++)
            {
                int value = int.Parse(loadedData.data1[i][j].ToString());
                if (value == 1)
                {
                    // Kiểm tra phía trên (i - 1) và phía trái (j - 1)
                    bool hasTopZero = (i == 0) || (i > 0 && int.Parse(loadedData.data1[i - 1][j].ToString()) == 0);
                    bool hasLeftZero = (j == 0) || (j > 0 && int.Parse(loadedData.data1[i][j - 1].ToString()) == 0);

                    Vector2 position = new Vector2(j, -i) + offset1;
                    GameObject cell = Instantiate(cellPrefabWoodModel, position, Quaternion.identity, gridData1);

                    // Đặt tên theo trường hợp
                    string nameSuffix = "normal";
                    BoxController boxController = cell.GetComponent<BoxController>();
                    if (hasTopZero && hasLeftZero)
                    {
                        nameSuffix = "top_left";
                        boxController.layerTop.SetActive(true);
                        boxController.layerLeft.SetActive(true);
                    }
                    else if (hasTopZero)
                    {
                        nameSuffix = "top";
                        boxController.layerTop.SetActive(true);
                    }
                    else if (hasLeftZero)
                    {
                        nameSuffix = "left";
                        boxController.layerLeft.SetActive(true);
                    }
                    cell.name = $"box_{i}_{j}_data1_{nameSuffix}";
                }
            }
        }
        StartCoroutine(MoveGrid(dropHandBox1, gridData1, new Vector2(offset1.x - 8,offset1.y + 8), offset1, timeDropHand));
    }
    private IEnumerator MoveGrid(DropHand hand, Transform grid, Vector2 start, Vector2 end, float duration)
    {
        float elapsedTime = 0;
        hand.ShowHand();

        // Bắt đầu từ vị trí ban đầu
        grid.position = start;
        while (elapsedTime < duration)
        {
            // Tính toán vị trí dựa trên thời gian đã trôi qua
            grid.position = Vector2.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null; // Đợi frame tiếp theo
        }

        // Đảm bảo vị trí cuối cùng chính xác
        grid.position = end;
        hand.ThaTay();

        // Di chuyển quay lại vị trí ban đầu
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            hand.targetHand.transform.position = Vector2.Lerp(end, start, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Đợi frame tiếp theo
        }

        // Đảm bảo vị trí cuối cùng chính xác
        hand.targetHand.transform.position = start;
        hand.CloseHand();
        hand.targetHand.transform.position = grid.position;
    }

    private void RecreateGridFromData2(BinaryJsonWrapper loadedData)
    {
        boxes.Clear();
        grid = new int[rows, cols];
        cellObjects = new GameObject[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int value = int.Parse(loadedData.data2[row][col].ToString());

                if (value == 1)
                {
                    grid[row, col] = 1;
                    Vector2 position = new Vector2(col, -row);

                    GameObject cell = Instantiate(cellPrefabWoodNormal, position, Quaternion.identity, gridData2);
                    cell.name = $"box_{row}_{col}_1";
                    cellObjects[row, col] = cell;

                    BoxController boxController = cell.GetComponent<BoxController>();
                    boxController.Initialize(row, col);
                    boxes.Add(boxController);
                }
                else if(value == 3)
                {
                    grid[row, col] = 1;
                    Vector2 position = new Vector2(col, -row);
                    GameObject cell = Instantiate(cellPrefabWoodScrew, position, Quaternion.identity, gridData2);
                    cell.name = $"box_{row}_{col}_2";
                    cellObjects[row, col] = cell;

                    BoxController boxController = cell.GetComponent<BoxController>();
                    boxController.Initialize(row, col);
                    boxes.Add(boxController);
                }    
            }
        }
        StartCoroutine(MoveGrid2(dropHandBox2, gridData2, new Vector2(8, 8), Vector2.zero, timeDropHand));
    }

    // data chứa box khi sử dùng hint
    private void RecreateGridFromData3(BinaryJsonWrapper loadedData)
    {
        boxes.Clear();
        grid = new int[rows, cols];
        cellObjects = new GameObject[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int value = int.Parse(loadedData.data3[row][col].ToString());

                if (value == 1)
                {
                    grid[row, col] = 1;
                    Vector2 position = new Vector2(col, -row);
                    GameObject cell = Instantiate(cellPrefabWoodNormal, position, Quaternion.identity, gridData2);
                    cell.name = $"box_{row}_{col}_1";
                    cellObjects[row, col] = cell;

                    BoxController boxController = cell.GetComponent<BoxController>();
                    boxController.Initialize(row, col);
                    boxes.Add(boxController);
                }
                else if (value == 2)
                {
                    grid[row, col] = 1;
                    Vector2 position = new Vector2(col, -row);
                    GameObject cell = Instantiate(cellPrefabWoodHint, position, Quaternion.identity, gridData2);
                    cell.name = $"box_{row}_{col}_2";
                    cellObjects[row, col] = cell;

                    BoxController boxController = cell.GetComponent<BoxController>();
                    boxController.Initialize(row, col);
                    boxes.Add(boxController);
                }
                else if (value == 3)
                {
                    grid[row, col] = 1;
                    Vector2 position = new Vector2(col, -row);
                    GameObject cell = Instantiate(cellPrefabWoodScrew, position, Quaternion.identity, gridData2);
                    cell.name = $"box_{row}_{col}_3";
                    cellObjects[row, col] = cell;

                    BoxController boxController = cell.GetComponent<BoxController>();
                    boxController.Initialize(row, col);
                    boxes.Add(boxController);
                }
            }
        }
        StartCoroutine(MoveGrid2(dropHandBox2, gridData2, new Vector2(8, 8), Vector2.zero, timeDropHand));
    }
    private IEnumerator MoveGrid2(DropHand hand, Transform grid, Vector2 start, Vector2 end, float duration)
    {
        float elapsedTime = 0;
        hand.ShowHand();

        // Bắt đầu từ vị trí ban đầu
        grid.position = start;
        while (elapsedTime < duration)
        {
            // Tính toán vị trí dựa trên thời gian đã trôi qua
            grid.position = Vector2.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null; // Đợi frame tiếp theo
        }

        // Đảm bảo vị trí cuối cùng chính xác
        grid.position = end;
        hand.ThaTay();

        // Di chuyển quay lại vị trí ban đầu
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            hand.targetHand.transform.position = Vector2.Lerp(end, start, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Đợi frame tiếp theo
        }

        // Đảm bảo vị trí cuối cùng chính xác
        hand.targetHand.transform.position = start;

        hand.CloseHand();

        hand.targetHand.transform.position = Vector2.zero;

    }
    private IEnumerator CreateWorm()
    {
        ClearWorm();
        yield return new WaitForSeconds(1f);

        Vector2 startPos = new Vector2(-7, -rows + 5); // Vị trí bắt đầu
        Vector2 targetPos = new Vector2(-3, -rows + 1); // Vị trí đích

        // Tạo con sâu ở vị trí bắt đầu
        GameObject wormInstance = Instantiate(wormController, startPos, Quaternion.identity);
        wormInstance.name = "WormController"; // Đặt tên để dễ tìm kiếm và quản lý

        Transform handTransform = WormController.Instance.handTarget.transform;
        Vector2 handStartPos = handTransform.position;
        Vector2 handReturnPos = new Vector2(handStartPos.x - 4, handStartPos.y);
        if (handTransform == null)
        {
            Debug.LogError("Không tìm thấy GameObject 'Hand' trong WormController");
            yield break;
        }

        // Di chuyển con sâu đến vị trí đích trong 1 giây
        yield return StartCoroutine(MoveToPosition(wormInstance.transform, targetPos, timeDropHand));

        WormController.Instance.handBat.SetActive(false);
        WormController.Instance.handTha.SetActive(true);
        yield return StartCoroutine(MoveToPosition(handTransform, handReturnPos, timeDropHand));
        WormController.Instance.handTarget.SetActive(false);

        isMoving = true; // Đặt trạng thái di chuyển thành true sau khi hoàn thành
        checkCreateMap = false;
        if(LevelNumber == 1) ShowTutorial();
    }
    private void ShowTutorial()
    {
        PopupManager.Instance.Show("tutorial");
    }    
    // Coroutine để di chuyển đối tượng từ vị trí hiện tại đến vị trí đích trong khoảng thời gian
    private IEnumerator MoveToPosition(Transform objTransform, Vector2 targetPos, float duration)
    {
        Vector2 startPos = objTransform.position;   
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration); // Tính tỉ lệ thời gian
            objTransform.position = Vector2.Lerp(startPos, targetPos, t); // Nội suy vị trí
            yield return null;
        }

        objTransform.position = targetPos; // Đảm bảo đối tượng đến chính xác vị trí đích
    }
    // xử lý thay đổi cạnh lề cho các box vừa bị ăn mất
    public void CheckLayerBot(int row,int col)
    {
        if(row < rows && cellObjects[row,col] != null)
            cellObjects[row,col].GetComponent<BoxController>().layerTop.SetActive(true);
    }
    public void CheckLayerRight(int row, int col)
    {
        if(col < cols && cellObjects[row,col] != null)
            cellObjects[row,col].GetComponent<BoxController>().layerLeft.SetActive(true);
    }

    [System.Serializable]
    public class BinaryJsonWrapper
    {
        public string[] data1; // Mảng dữ liệu thứ nhất
        public string[] data2; // Mảng dữ liệu thứ hai
        public string[] data3; // Mảng dữ liệu thứ ba (hint)

        public BinaryJsonWrapper(string[] data1, string[] data2, string[] data3)
        {
            this.data1 = data1;
            this.data2 = data2;
            this.data3 = data3;
        }
    }
    public Dictionary<string, object> Save()
    {
        // Create the main save data json object
        Dictionary<string, object> json = new Dictionary<string, object>();

        //json["version"] = SaveVersion;
        json["level_number"] = LevelNumber;
        json["coins"] = Coins;
        return json;
    }
    private bool LoadSave()
    {
        //levelSaveDatas = new Dictionary<string, LevelSaveData>();

        // Changed in 1.4 to use ISaveable instead, if SaveFilePath exists then we load it and delete it so the new save system is used next time
        if (System.IO.File.Exists(SaveFilePath))
        {
            JSONNode json = JSON.Parse(System.IO.File.ReadAllText(SaveFilePath));

            ParseSaveData(json);

            System.IO.File.Delete(SaveFilePath);

            return true;
        }
        else if(SaveManager.Exists())
        {
            JSONNode json  = SaveManager.Instance.LoadSave(this);
            if(json == null)
            {
                return false;
            }
            ParseSaveData(json);
            return true;
        }

        return false;
    }
    private void ParseSaveData(JSONNode json)
    {
        LevelNumber = json["level_number"].AsInt;
        Coins = json["coins"].AsInt;
    }
    public void GiveCoins(int coins)
    {
        Coins += coins;
    }
    public override void OnDestroy()
    {
        Save();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
    }
    public bool AreGridsMatching()
    {
        HashSet<Vector2Int> gridParentPositions = new HashSet<Vector2Int>();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (grid[row, col] == 1) // Nếu box còn tồn tại
                {
                    gridParentPositions.Add(new Vector2Int(row, col));
                }
                else if (grid[row, col] == 2) // Nếu phát hiện giá trị 2, kết thúc kiểm tra
                {
                    //Debug.LogError("Grid contains invalid value (2), cannot match!");
                    return false;
                }
            }
        }

        // 2. Lấy danh sách các vị trí box từ file JSON
        HashSet<Vector2Int> fileBoxPositions = new HashSet<Vector2Int>();

        if (listDataLevel != null)
        {
            // Đọc nội dung từ file JSON được gắn
            string jsonData = listDataLevel[levelSelect].text;
            BinaryJsonWrapper loadedData = JsonUtility.FromJson<BinaryJsonWrapper>(jsonData);

            if (loadedData != null && loadedData.data1 != null)
            {
                int fileRows = loadedData.data1.Length;
                int fileCols = loadedData.data1[0].Length;

                // Duyệt qua mảng data1 để lấy các vị trí box
                for (int i = 0; i < fileRows; i++)
                {
                    for (int j = 0; j < fileCols; j++)
                    {
                        if (loadedData.data1[i][j] == '1') // Nếu giá trị là '1'
                        {
                            fileBoxPositions.Add(new Vector2Int(i, j));
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Dữ liệu JSON không hợp lệ hoặc mảng data1 không tồn tại!");
                return false;
            }
        }
        else
        {
            Debug.LogError("Chưa gắn file JSON vào Inspector!");
            return false;
        }

        // 3. So sánh hai danh sách
        if (gridParentPositions.SetEquals(fileBoxPositions))
        {
            isMoving = false;

            Debug.LogError("win");
            StartCoroutine(WinGame());

            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator WinGame()
    {
        yield return new WaitForSeconds(0.2f);
        SoundManager.Instance.Play("done");
        if (LevelNumber == UIManager.Instance.levelCurrent)
        {
            LevelNumber ++;
        }
        yield return new WaitForSeconds(1f);
        PopupManager.Instance.Show("complete");
        SoundManager.Instance.Play("win");
    }
    public void LoseGame()
    {
        PopupManager.Instance.Show("lose");
        SoundManager.Instance.Play("lose");
    }
    public bool IsPositionBlocked(Vector2 position)
    {
        Vector2Int gridPos = Vector2Int.RoundToInt(new Vector2(position.x, -position.y));
        int row = gridPos.y;
        int col = gridPos.x;

        if (row >= 0 && row < rows && col >= 0 && col < cols)
        {
            return grid[row, col] != 0;
        }
        return false;
    }

    private void FloodFillAdvanced(int row, int col, bool[,] visited, int[,] labels, int label)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols || visited[row, col] || grid[row, col] == 0)
            return;

        visited[row, col] = true;
        labels[row, col] = label;

        if (col - 1 >= 0) // Cột bên trái
        {
            if (row - 1 >= 0) FloodFillAdvanced(row - 1, col - 1, visited, labels, label); // Chéo trên trái
            FloodFillAdvanced(row, col - 1, visited, labels, label);                      // Thẳng trái
            if (row + 1 < rows) FloodFillAdvanced(row + 1, col - 1, visited, labels, label); // Chéo dưới trái
        }
        if (col + 1 < cols) // Cột bên phải
        {
            if (row - 1 >= 0) FloodFillAdvanced(row - 1, col + 1, visited, labels, label); // Chéo trên phải
            FloodFillAdvanced(row, col + 1, visited, labels, label);                      // Thẳng phải
            if (row + 1 < rows) FloodFillAdvanced(row + 1, col + 1, visited, labels, label); // Chéo dưới phải
        }
        // Tiếp tục flood fill đến các ô xung quanh
        FloodFillAdvanced(row - 1, col, visited, labels, label); // Lên
        FloodFillAdvanced(row + 1, col, visited, labels, label); // Xuống
        FloodFillAdvanced(row, col - 1, visited, labels, label); // Trái
        FloodFillAdvanced(row, col + 1, visited, labels, label); // Phải
    }

    public void HandleDisconnectedPiecesAdvanced()
    {
        if (!createdBox)
        {
            return;
        }
        bool[,] visited = new bool[rows, cols];
        int[,] labels = new int[rows, cols];
        int currentLabel = 1;

        // Gán nhãn cho các nhóm liên kết
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (grid[row, col] == 1 && !visited[row, col])
                {
                    //Debug.Log($"FloodFillAdvanced: Starting new group at ({row}, {col}) with label {currentLabel}");
                    FloodFillAdvanced(row, col, visited, labels, currentLabel);
                    currentLabel++;
                }
            }
        }

        // Kiểm tra từng nhóm có được nối đất hay không
        for (int label = 1; label < currentLabel; label++)
        {
            if (!IsGrounded(label, labels))
            {
                DropFloatingPieces(label, labels);
            }
        }
    }

    private bool IsGrounded(int label, int[,] labels)
    {
        for (int col = 0; col < cols; col++)
        {
            if (labels[rows - 1, col] == label)
            {
                return true;
            }
        }
        return false;
    }
    // kiem tra vi tri sau
    public bool IsWormBelow(Vector2 position)
    {
        Vector2 belowPosition = position + Vector2.down;
        return WormController.Instance.IsWormAtPosition(belowPosition);
    }
    private void DropFloatingPieces(int label, int[,] labels)
    {
        bool hasMoved;
        bool shouldStop = false;

        do
        {
            hasMoved = false;

            // Kiểm tra tất cả các box trước khi xử lý
            for (int row = rows - 1; row >= 0; row--) // Duyệt từ dưới lên trên
            {
                for (int col = 0; col < cols; col++)
                {
                    if (labels[row, col] == label)
                    {                      
                        // Kiểm tra vị trí phía dưới của box
                        Vector2 currentPosition = new Vector2(col, - row);
                        if (IsWormBelow(currentPosition))
                        {
                            shouldStop = true; // Nếu có sâu phía dưới bất kỳ box nào, dừng rơi
                        }
                    }
                }
            }

            // Nếu phát hiện có sâu phía dưới, dừng toàn bộ quá trình rơi
            if (shouldStop)
            {
                return; // Không xử lý rơi, chờ lần kiểm tra tiếp theo
            }
            // Nếu không có sâu phía dưới, tiếp tục xử lý rơi
            for (int row = rows - 1; row >= 0; row--) // Duyệt từ dưới lên trên
            {
                for (int col = 0; col < cols; col++)
                {
                    if (labels[row, col] == label)
                    {
                        bool canDrop = false;

                        // Kiểm tra ô dưới
                        if (row + 1 < rows && grid[row + 1, col] == 0)
                        {
                            //Debug.LogError("222");
                            canDrop = true;
                        }

                        if (canDrop)
                        {
                            int targetRow = row + 1;

                            // Nếu ô dưới rỗng, box rơi xuống
                            if (targetRow < rows && grid[targetRow, col] == 0)
                            {
                                // Cập nhật trạng thái lưới
                                grid[targetRow, col] = grid[row, col];
                                grid[row, col] = 0;

                                // Cập nhật trạng thái cellObject
                                cellObjects[targetRow, col] = cellObjects[row, col];
                                cellObjects[row, col] = null;

                                // Di chuyển box xuống 1 ô trong không gian
                                Vector2 newPosition = new Vector2(col, -targetRow);
                                cellObjects[targetRow, col].transform.position = newPosition;

                                hasMoved = true;
                            }
                        }
                    }
                }
            }
        }
        while (hasMoved); // Lặp lại quá trình nếu còn box có thể rơi
    }
}
