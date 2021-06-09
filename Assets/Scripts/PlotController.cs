using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlotController : MonoBehaviour
{
    public Canvas canvas;
    public GameController gameController;
    public GameObject messagePrefab;
    public Button neutrophil;
    public Button nkCell;
    public Button macrophage;
    public Button dendriticCell;
    public Button tKiller;
    private GameObject messageObject;
    private bool temperatureHasRisen;

    public IEnumerable<Action> PlotActions()
    {
        neutrophil.interactable = false;
        nkCell.interactable = false;
        macrophage.interactable = false;
        dendriticCell.interactable = false;
        tKiller.interactable = false;

        yield return InnateImmunityInfo(LymphnodeInfo(ProteinInfo()));

        foreach (var action in ThreatKillingCycle(
            () => gameController.SpawnThreat(new Vector2(3, -1.5f), new ThreatData(0, "Первая ранка", ThreatType.Wound),
                1),
            WoundInfo(NeutrophilInfo())))
            yield return action;

        yield return NkCellInfo();

        foreach (var action in ThreatKillingCycle(
            () => gameController.SpawnThreat(new Vector2(1.5f, 3), new ThreatData(1, "Первый вирус", ThreatType.Virus), 1),
            VirusInfo()))
            yield return action;

        if (!temperatureHasRisen) yield return TemperatureInfo();

        yield return AcquiredImmunityInfo();

        yield return () => gameController.SpawnThreat(new Vector2(-4.5f, -1.5f),
            new ThreatData(2, "Второй вирус", ThreatType.Virus), 1);

        yield return AntiBodiesAction(MacrophageInfo(DendriticCellInfo()));

        while (gameController.ThreatsWithAntiBodiesCodes.Count == 0)
            if (gameController.Threats.Count == 0)
                yield return () => gameController.SpawnThreat(new Vector2(-4.5f, -1.5f),
                    new ThreatData(2, "Второй вирус", ThreatType.Virus), 1);
            else
                yield return () => { };

        yield return KillerInfo();
        yield return () => gameController.plotMode = false;
    }

    private void ShowMessage(string message, float x, float y, Color color, UnityAction buttonAction)
    {
        HideMessage();
        Time.timeScale = 0;
        messageObject = Instantiate(messagePrefab, new Vector3(0, 0, 0), new Quaternion(), canvas.transform);
        messageObject.GetComponentInChildren<Text>().text = message;
        messageObject.GetComponentInChildren<Button>().onClick.AddListener(buttonAction);
        messageObject.GetComponent<MessageScript>().frame.color = color;
        messageObject.transform.localPosition = new Vector3(x, y);
    }

    private void HideMessage()
    {
        Destroy(messageObject);
        Time.timeScale = 1;
    }

    private Action InnateImmunityInfo(UnityAction lymphnodeInfo)
    {
        return () => ShowMessage(
            "Врождённый иммунитет — это способность организма обезвреживать чужеродный и потенциально опасный биоматериал существующая изначально, до первого попадания этого биоматериала в организм.",
            0, 0, new Color(0.36f, 0.70f, 0.38f, 1), () => ShowMessage(
                "Врожденный иммунитет реагирует сразу же после проникновения микроба в организм и не формирует иммунологическую память.",
                0, 0, new Color(0.36f, 0.70f, 0.38f, 1), () => ShowMessage(
                    "Врожденный иммунитет очень важен, потому что он первый сигнализирует об опасности и немедленно начинает давать отпор проникшим микробам.",
                    0, 0, new Color(0.36f, 0.70f, 0.38f, 1), lymphnodeInfo)));
    }

    private UnityAction LymphnodeInfo(UnityAction proteinInfo)
    {
        return () => ShowMessage(
            "В лимфатических узлах находятся клетки памяти — это специальные клетки иммунной системы, которые хранят информацию о микробах, уже проникавших в организм ранее.",
            -110, -90, new Color(0.36f, 0.70f, 0.38f, 1), () => ShowMessage(
                "По своему строению они напоминают губку, через которую постоянно фильтруется лимфа",
                -110, -90, new Color(0.36f, 0.70f, 0.38f, 1), () => ShowMessage(
                    "В порах этой губки очень много иммунных клеток, которые ловят и переваривают микробов, проникших в организм, и сохраняют о них информацию.",
                    -110, -90, new Color(0.36f, 0.70f, 0.38f, 1), proteinInfo)));
    }

    private UnityAction ProteinInfo()
    {
        return () => ShowMessage(
            "Одна из важнейших функций, которую выполняют белки в организме, — защитная.",
            50, 110, new Color(0.98f, 0.57f, 0.32f, 1), () => ShowMessage(
                "Ядро иммунной системы составляют три типа белков: антитела, интерфероны и белки главного комплекса гистосовместимости (большая область генома или большое семейство генов, обнаруженное у позвоночных и играющее важную роль в иммунной системе и развитии иммунитета).",
                50, 110, new Color(0.98f, 0.57f, 0.32f, 1), HideMessage));
    }

    private Action WoundInfo(UnityAction neutrophilInfo)
    {
        return () =>
            ShowMessage(
                "Раны, при которых повреждены только кожа и слизистые оболочки, называются поверхностными. Если повреждение распространяется на расположенные глубже, то раны считаются глубокими.",
                -80, -125, new Color(0.98f, 0.86f, 0.69f, 1), () =>
                    ShowMessage(
                        "Все раны (кроме операционных) первично загрязнены микробами. Однако развитие инфекционного процесса в организме наблюдается не при всех ранениях.",
                        -80, -125, new Color(0.98f, 0.86f, 0.69f, 1), () =>
                            ShowMessage(
                                "Классификация раневых инфекций: инфекция раны, околораневой абсцесс, раневая флегмона, гнойный затек, свищ, тромбофлебит, лимфангит и лимфаденит.",
                                -80, -125, new Color(0.98f, 0.86f, 0.69f, 1), () =>
                                {
                                    gameController.Threats.FirstOrDefault()?.GetComponent<Threat>().ActivateThreat();
                                    neutrophil.interactable = true;
                                    neutrophilInfo();
                                })));
    }

    private UnityAction NeutrophilInfo()
    {
        return () =>
            ShowMessage(
                "Нейтрофилы являются частью врождённого иммунитета, их основная функция — фагоцитоз (процесс, при котором клетки захватывают и переваривают твёрдые частицы) патогенных микроорганизмов и продуктов распада тканей организма.",
                50, -65, new Color(0.93f, 0.38f, 0.38f, 1), () =>
                    ShowMessage(
                        "Нейтрофилы представляют собой очень подвижные клетки, которые проникают даже в те ткани, которые недоступны для других лейкоцитов.",
                        50, -65, new Color(0.93f, 0.38f, 0.38f, 1), () =>
                            ShowMessage(
                                "Воспалённые или повреждённые участки соединительной ткани требуют немедленной миграции нейтрофилов в очаг повреждения для удаления патогенных микроорганизмов и восстановления ткани.",
                                50, -65, new Color(0.93f, 0.38f, 0.38f, 1), HideMessage)));
    }

    private Action TemperatureInfo()
    {
        return () =>
            ShowMessage("Температура поднимается", 30, 187, new Color(0.6f, 0.6f, 0.6f, 1), HideMessage);
    }

    private Action NkCellInfo()
    {
        nkCell.interactable = true;
        return () =>
            ShowMessage(
                "Натуральные киллеры – это тип Т-киллеров, участвующий в функционировании врождённого иммунитета.",
                50, -120, new Color(0.27f, 0.40f, 0.80f, 1), () =>
                    ShowMessage(
                        "NK-клетки клетки находят и убивают раковые клетки и клетки, пораженные вирусами",
                        50, -120, new Color(0.27f, 0.40f, 0.80f, 1), HideMessage));
    }

    private Action VirusInfo()
    {
        return () =>
            ShowMessage(
                "Вирус — неклеточный инфекционный агент, который может воспроизводиться только внутри клеток.",
                -145, 115, new Color(0.36f, 0.70f, 0.38f, 1), () =>
                    ShowMessage(
                        "Примерами наиболее известных вирусных заболеваний человека могут служить простуда, грипп, ветряная оспа и простой герпес.",
                        -145, 115, new Color(0.36f, 0.70f, 0.38f, 1), () =>
                            ShowMessage(
                                "Хотя вирусы подрывают нормальный гомеостаз (саморегуляция), приводя к заболеванию, они могут существовать внутри организма и относительно безвредно. Некоторые вирусы могут пребывать внутри тела человека в состоянии покоя.",
                                -145, 115, new Color(0.36f, 0.70f, 0.38f, 1), HideMessage)));
    }

    private Action AcquiredImmunityInfo()
    {
        return () => ShowMessage(
            "Приобретённый иммунитет — это способность организма обезвреживать чужеродные и потенциально опасные микроорганизмы, которые уже попадали в организм ранее.",
            0, 0, new Color(0.36f, 0.70f, 0.38f, 1), () => ShowMessage(
                "Реакция организма индивидуальна для каждого «врага», поэтому «арсенал» приобретенного иммунитета зависит от того, с какими инфекциями человек сталкивался в жизни и какие прививки делал.",
                0, 0, new Color(0.36f, 0.70f, 0.38f, 1), HideMessage));
    }

    private Action AntiBodiesAction(UnityAction macrophageInfo)
    {
        return () =>
            ShowMessage(
                "Антитела́ — крупные глобулярные белки плазмы крови, выделяющиеся плазматическими клетками иммунной системы и предназначенные для нейтрализации клеток патогенов (бактерий, грибов, многоклеточных паразитов) и вирусов.",
                10, -130, new Color(0.36f, 0.70f, 0.38f, 1), () =>
                    ShowMessage(
                        "Каждое антитело распознаёт уникальный элемент патогена, отсутствующий в самом организме, — антиген.",
                        10, -130, new Color(0.36f, 0.70f, 0.38f, 1), () =>
                            ShowMessage(
                                "Антитела поступают в кровь, разносятся по всему организму и связываются со всеми проникшими бактериями, вызывая их гибель.",
                                10, -130, new Color(0.36f, 0.70f, 0.38f, 1), () =>
                                {
                                    macrophage.interactable = true;
                                    macrophageInfo();
                                })));
    }

    private UnityAction MacrophageInfo(UnityAction dendriticCellInfo)
    {
        return () =>
            ShowMessage(
                "Макрофаги — это клетки в организме человека, способные к активному захвату и перевариванию бактерий, остатков погибших клеток и других чужеродных или токсичных для организма частиц.",
                50, 35, new Color(0.49f, 0.78f, 0.85f, 1), () =>
                    ShowMessage(
                        "Макрофаг знакомит другие клетки иммунной системы с кусочками переваренного микроба, что позволяет организму лучше бороться с инфекцией.",
                        50, 35, new Color(0.49f, 0.78f, 0.85f, 1), () =>
                            ShowMessage(
                                "Макрофаги присутствуют практически в каждом органе и ткани, где они выступают в качестве первой линии иммунной защиты от патогенов и играют важную роль в поддержании тканевого гомеостаза (саморегуляции).",
                                50, 35, new Color(0.49f, 0.78f, 0.85f, 1), () =>
                                {
                                    dendriticCell.interactable = true;
                                    dendriticCellInfo();
                                })));
    }

    private UnityAction DendriticCellInfo()
    {
        return () => ShowMessage(
            "Дендритные клетки",
            50, -10, new Color(0.87f, 0.58f, 0.76f, 1), HideMessage);
    }

    private Action KillerInfo()
    {
        return () =>
            ShowMessage(
                "Т-лимфоциты так названы, потому что после образования в костном мозге дозревают в вилочковой железе — тимусе.",
                50, -165, new Color(0.19f, 0.53f, 0.47f, 1), () =>
                {
                    tKiller.interactable = true;
                    ShowMessage(
                        "Есть три подвида Т-лимфоцитов: Т-киллеры, Т-хелперы и Т-супрессоры",
                        50, -165, new Color(0.19f, 0.53f, 0.47f, 1), () =>
                            ShowMessage(
                                "Т-киллеры могут убивать зараженные вирусами клетки, чтобы остановить развитие инфекции.",
                                50, -165, new Color(0.19f, 0.53f, 0.47f, 1), HideMessage));
                });
    }

    private IEnumerable<Action> ThreatKillingCycle(Action spawnThreat, Action info)
    {
        gameController.FirstThreatWin = false;
        yield return spawnThreat;
        yield return info;
        while (!gameController.FirstThreatWin)
            if (gameController.Threats.Count == 0)
            {
                if (!temperatureHasRisen)
                {
                    temperatureHasRisen = true;
                    yield return TemperatureInfo();
                }

                yield return spawnThreat;
            }
            else
            {
                yield return () => { };
            }
    }
}