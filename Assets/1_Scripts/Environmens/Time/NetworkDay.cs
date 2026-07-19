using System;
using MeowTruck;
using MeowTruck.Manager;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkDay : NetworkBehaviour
{
    public static NetworkDay Instance { get; private set; }

    public enum DayPeriod : byte
    {
        Day,
        Night
    }

    [Header("Cycle")]
    [SerializeField, Min(1f)] private float cycleLengthSeconds = 600f;

    public NetworkVariable<double> CycleStartedAt = new();  // 시간 진행을 시작한 서버 시각
    public NetworkVariable<float> CyclePosition = new(0f);  // 진행을 멈췄을 때 저장한 낮/밤 위치
    public NetworkVariable<int> DayNumber = new(1);         // 현재 날짜
    public NetworkVariable<bool> IsRunning = new(false);    // 현재 시간이 흐르는지 여부

    public event Action<DayPeriod> PeriodChanged;

    private DayPeriod lastPeriod;
    private string lastGameplaySceneName;
    private bool foodTruckExitRequested;

    private void Awake()
    {
        Instance = this;
    }

    public float NormalizedTime
    {
        get
        {
            if (!IsSpawned || !IsRunning.Value)
                return CyclePosition.Value;

            double elapsed = NetworkManager.ServerTime.Time - CycleStartedAt.Value;
            return (float)((CyclePosition.Value + elapsed / cycleLengthSeconds) % 1.0);
        }
    }

    public DayPeriod CurrentPeriod => NormalizedTime < 0.5f
        ? DayPeriod.Day
        : DayPeriod.Night;

    public float PeriodProgress => CurrentPeriod == DayPeriod.Day
        ? NormalizedTime * 2f
        : (NormalizedTime - 0.5f) * 2f;

    public override void OnNetworkSpawn()
    {
        lastPeriod = CurrentPeriod;

        if (!IsServer)
            return;

        SceneManager.activeSceneChanged += HandleActiveSceneChanged;
        ApplySceneTimeState(SceneManager.GetActiveScene());
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
            SceneManager.activeSceneChanged -= HandleActiveSceneChanged;

        if (Instance == this)
            Instance = null;

        base.OnNetworkDespawn();
    }

    public override void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        if (!IsSpawned || !IsRunning.Value)
            return;

        DayPeriod currentPeriod = CurrentPeriod;
        if (currentPeriod == lastPeriod)
            return;

        lastPeriod = currentPeriod;
        PeriodChanged?.Invoke(currentPeriod);

        if (IsServer && currentPeriod == DayPeriod.Day)
            DayNumber.Value++;

        if (IsServer && SceneManager.GetActiveScene().name == Constants.SCENE_FOOD_TRUCK)
        {
            FinishFoodTruckPeriod(currentPeriod);
        }
    }

    private void HandleActiveSceneChanged(Scene previousScene, Scene nextScene)
    {
        ApplySceneTimeState(nextScene);
    }

    private void ApplySceneTimeState(Scene scene)
    {
        string sceneName = scene.name;

        if (sceneName == Constants.SCENE_VILLAGE)
        {
            PauseCycle();

            if (lastGameplaySceneName == Constants.SCENE_FIELD)
                AdvanceToNextPeriod();

            lastGameplaySceneName = Constants.SCENE_VILLAGE;
            foodTruckExitRequested = false;
            return;
        }

        if (sceneName == Constants.SCENE_FOOD_TRUCK)
        {
            lastGameplaySceneName = Constants.SCENE_FOOD_TRUCK;
            foodTruckExitRequested = false;

            PauseCycle();

            return;
        }

        if (sceneName == Constants.SCENE_FIELD)
        {
            lastGameplaySceneName = Constants.SCENE_FIELD;
            PauseCycle();
            return;
        }

        PauseCycle();
    }

    public bool StartFoodTruckPeriod()
    {
        if (!IsServer || SceneManager.GetActiveScene().name != Constants.SCENE_FOOD_TRUCK)
            return false;

        if (IsRunning.Value)
            return false;

        foodTruckExitRequested = false;
        ResumeCycle();
        return true;
    }

    private void ResumeCycle()
    {
        if (!IsServer || IsRunning.Value)
            return;

        CycleStartedAt.Value = NetworkManager.ServerTime.Time;
        IsRunning.Value = true;
        lastPeriod = CurrentPeriod;
        PeriodChanged?.Invoke(lastPeriod);
    }

    private void PauseCycle()
    {
        if (!IsServer || !IsRunning.Value)
            return;

        CyclePosition.Value = NormalizedTime;
        IsRunning.Value = false;
    }

    private void FinishFoodTruckPeriod(DayPeriod arrivedPeriod)
    {
        if (foodTruckExitRequested)
            return;

        foodTruckExitRequested = true;
        PauseCycle();

        CyclePosition.Value = arrivedPeriod == DayPeriod.Day ? 0f : 0.5f;
        NetworkManager.SceneManager.LoadScene(Constants.SCENE_VILLAGE, LoadSceneMode.Single);
    }

    private void AdvanceToNextPeriod()
    {
        DayPeriod nextPeriod = CurrentPeriod == DayPeriod.Day
            ? DayPeriod.Night
            : DayPeriod.Day;

        CyclePosition.Value = nextPeriod == DayPeriod.Day ? 0f : 0.5f;
        CycleStartedAt.Value = NetworkManager.ServerTime.Time;
        IsRunning.Value = false;

        if (nextPeriod == DayPeriod.Day)
            DayNumber.Value++;

        lastPeriod = nextPeriod;
        PeriodChanged?.Invoke(nextPeriod);
    }
}

