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
    public GameObject lymphnode;
    private GameObject messageObject;
    private bool temperatureHasRisen;

    public IEnumerable<Action> PlotActions()
    {
        lymphnode.SetActive(false);
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
            () => gameController.SpawnThreat(new Vector2(1.5f, 3), new ThreatData(1, "Первый вирус", ThreatType.Virus),
                1),
            VirusInfo()))
            yield return action;

        if (!temperatureHasRisen) yield return TemperatureInfo();

        yield return AcquiredImmunityInfo();

        yield return () => gameController.SpawnThreat(new Vector2(-4.5f, -1.5f),
            new ThreatData(2, "Второй вирус", ThreatType.Virus), 1);

        yield return AntiBodiesAction(MacrophageInfo(DendriticCellInfo()));

        while (gameController.ThreatsWithAntiBodiesCodes.Count == 0)
            if (gameController.threats.Count == 0)
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
        UIManagerScript.PauseGame = true;
        messageObject = Instantiate(messagePrefab, new Vector3(0, 0, 0), new Quaternion(), canvas.transform);
        messageObject.GetComponentInChildren<Text>().text = message;
        messageObject.GetComponentInChildren<Button>().onClick.AddListener(buttonAction);
        messageObject.GetComponent<MessageScript>().frame.color = color;
        messageObject.transform.localPosition = new Vector3(x, y);
    }

    private void HideMessage()
    {
        Destroy(messageObject);
        UIManagerScript.PauseGame = false;
    }

    private Action InnateImmunityInfo(UnityAction lymphnodeInfo)
    {
        return () => ShowMessage(
            "Врождённый иммунитет — это способность организма обезвреживать чужеродный и потенциально опасный биоматериал существующая изначально, до первого попадания этого биоматериала в организм.",
            0, 0, new Color(0.36f, 0.70f, 0.38f, 1), () => ShowMessage(
                "Врожденный иммунитет реагирует сразу же после проникновения микроба в организм и не формирует иммунологическую память.",
                0, 0, new Color(0.36f, 0.70f, 0.38f, 1), () => ShowMessage(
                    "Врожденный иммунитет очень важен, потому что он первый сигнализирует об опасности и немедленно начинает давать отпор проникшим микробам.",
                    0, 0, new Color(0.36f, 0.70f, 0.38f, 1), () => ShowMessage(
                        "В нашей игре вам придется взять на себя управление человеческим иммунитетом. Ваше цель – защитить свое здоровье любой ценой!",
                        0, 0, new Color(0.36f, 0.70f, 0.38f, 1), lymphnodeInfo))));
    }

    private UnityAction LymphnodeInfo(UnityAction proteinInfo)
    {
        return () =>
        {
            lymphnode.SetActive(true);
            ShowMessage(
                "Круг в центре экрана – лимфоузел, один из ключевых объектов игры.",
                -250, -200, new Color(0.36f, 0.70f, 0.38f, 1), () => ShowMessage(
                    "В лимфатических узлах находятся клетки памяти — это специальные клетки иммунной системы, которые хранят информацию о микробах, уже проникавших в организм ранее.",
                    -250, -200, new Color(0.36f, 0.70f, 0.38f, 1), () => ShowMessage(
                        "По своему строению они напоминают губку, через которую постоянно фильтруется лимфа",
                        -250, -200, new Color(0.36f, 0.70f, 0.38f, 1), () => ShowMessage(
                            "В порах этой губки очень много иммунных клеток, которые ловят и переваривают микробов, проникших в организм, и сохраняют о них информацию.",
                            -250, -200, new Color(0.36f, 0.70f, 0.38f, 1), proteinInfo))));
        };
    }

    private UnityAction ProteinInfo()
    {
        return () => ShowMessage(
            "Для создания иммунных тел организму нужны ресурсы. Наиболее важный – белок – вы видите справа. В реальной жизни человек усваивает белок при потреблении пищи, в нашей игре он просто накапливается со временем.",
            150, 150, new Color(0.98f, 0.57f, 0.32f, 1), HideMessage);
    }

    private Action WoundInfo(UnityAction neutrophilInfo)
    {
        return () =>
            ShowMessage(
                "Иммунитет распознал первую угрозу – раневую инфекцию. ",
                -150, -200, new Color(0.98f, 0.86f, 0.69f, 1), () =>
                    ShowMessage(
                        "Раны, при которых повреждены только кожа и слизистые оболочки, называются поверхностными. Если повреждение распространяется на расположенные глубже, то раны считаются глубокими.",
                        -150, -200, new Color(0.98f, 0.86f, 0.69f, 1), () =>
                            ShowMessage(
                                "Все раны (кроме операционных) первично загрязнены микробами. Однако развитие инфекционного процесса в организме наблюдается не при всех ранениях.",
                                -150, -200, new Color(0.98f, 0.86f, 0.69f, 1), () =>
                                    ShowMessage(
                                        "Красная шкала под угрозой – шкала ее здоровья. Для победы над угрозой необходимо опустить эту шкалу до нуля. Но стоит поторопиться, потому что, если таймер вокруг угрозы истечет раньше этого, будут последствия!",
                                        -150, -200, new Color(0.98f, 0.86f, 0.69f, 1), () =>
                                            ShowMessage(
                                                "Нажмите на угрозу, чтобы выделить ее.",
                                                -150, -200, new Color(0.98f, 0.86f, 0.69f, 1), () =>
                                                {
                                                    gameController.threats.FirstOrDefault()?.GetComponent<Threat>()
                                                        .ActivateThreat();
                                                    neutrophil.interactable = true;
                                                    neutrophilInfo();
                                                })))));
    }

    private UnityAction NeutrophilInfo()
    {
        return () =>
            ShowMessage(
                "Нейтрофилы являются частью врождённого иммунитета, их основная функция — фагоцитоз (процесс, при котором клетки захватывают и переваривают твёрдые частицы) патогенных микроорганизмов и продуктов распада тканей организма.",
                150, -130, new Color(0.93f, 0.38f, 0.38f, 1), () =>
                    ShowMessage(
                        "Нейтрофилы представляют собой очень подвижные клетки, которые проникают даже в те ткани, которые недоступны для других лейкоцитов.",
                        150, -130, new Color(0.93f, 0.38f, 0.38f, 1), () =>
                            ShowMessage(
                                "Нейтрофилы – первый тип иммунных тел, которые мы дадим под ваш контроль. Для того, чтобы отправить нейтрофила к выбранной угрозе, кликните по нему (это заберет часть белка).\nНе забудьте предварительно закрыть эту справку.",
                                150, -130, new Color(0.93f, 0.38f, 0.38f, 1), HideMessage)));
    }

    private Action TemperatureInfo()
    {
        return () =>
            ShowMessage(
                "Если вы не успели обезвредить угрозу до окончания таймера, организму придется сделать это, жертвуя вашим комфортом – повышать температуру тела.",
                110, 270, new Color(0.6f, 0.6f, 0.6f, 1), () =>
                    ShowMessage(
                        "Небольшое повышение температуры не смертельно, да и со временем температура падает до нормы. Но аккуратнее, если температура повысится до 40 градусов – вы проиграете.",
                        110, 270, new Color(0.6f, 0.6f, 0.6f, 1), HideMessage));
    }

    private Action NkCellInfo()
    {
        nkCell.interactable = true;
        return () =>
            ShowMessage(
                "Натуральные киллеры – еще один тип иммунных тел, участвующий в функционировании врождённого иммунитета.",
                150, -210, new Color(0.27f, 0.40f, 0.80f, 1), () =>
                    ShowMessage(
                        "NK-клетки клетки находят и убивают раковые клетки и клетки, пораженные вирусами.",
                        150, -210, new Color(0.27f, 0.40f, 0.80f, 1), () =>
                            ShowMessage(
                                "Испытайте NK-клетки на следующей угрозе. Заметьте, что они дороже, сильнее, но медленнее нейтрофилов. ",
                                150, -210, new Color(0.27f, 0.40f, 0.80f, 1), HideMessage)));
    }

    private Action VirusInfo()
    {
        return () =>
            ShowMessage(
                "Вирус — неклеточный инфекционный агент, который может воспроизводиться только внутри клеток.",
                -270, 260, new Color(0.36f, 0.70f, 0.38f, 1), () =>
                    ShowMessage(
                        "Примерами наиболее известных вирусных заболеваний человека могут служить простуда, грипп, ветряная оспа и простой герпес.",
                        -270, 260, new Color(0.36f, 0.70f, 0.38f, 1), HideMessage));
    }

    private Action AcquiredImmunityInfo()
    {
        return () =>
        {
            lymphnode.SetActive(false);
            ShowMessage(
                "Приобретённый иммунитет — это способность организма обезвреживать чужеродные и потенциально опасные микроорганизмы, которые уже попадали в организм ранее.",
                0, 0, new Color(0.36f, 0.70f, 0.38f, 1), () => ShowMessage(
                    "Реакция организма индивидуальна для каждого «врага», поэтому «арсенал» приобретенного иммунитета зависит от того, с какими инфекциями человек сталкивался в жизни и какие прививки делал.",
                    0, 0, new Color(0.36f, 0.70f, 0.38f, 1), () =>
                    {
                        lymphnode.SetActive(true);
                        HideMessage();
                    }));
        };
    }

    private Action AntiBodiesAction(UnityAction macrophageInfo)
    {
        return () =>
        {
            ShowMessage(
                "Антитела — крупные глобулярные белки плазмы крови, выделяющиеся плазматическими клетками иммунной системы и предназначенные для нейтрализации клеток патогенов (бактерий, грибов, многоклеточных паразитов) и вирусов.",
                30, -130, new Color(0.36f, 0.70f, 0.38f, 1), () =>
                    ShowMessage(
                        "Каждое антитело распознаёт уникальный элемент патогена, отсутствующий в самом организме, — антиген.",
                        30, -130, new Color(0.36f, 0.70f, 0.38f, 1), () =>
                            ShowMessage(
                                "Зеленая шкала – уровень антигена угрозы. Если опустить ее до нуля, вы получите антитела к угрозе. Если позже в организм попадет та же угроза, антитела к ней сохранятся.",
                                30, -130, new Color(0.36f, 0.70f, 0.38f, 1), () =>
                                {
                                    macrophage.interactable = true;
                                    macrophageInfo();
                                })));
        };
    }

    private UnityAction MacrophageInfo(UnityAction dendriticCellInfo)
    {
        return () =>
            ShowMessage(
                "Макрофаги — это клетки в организме человека, способные к активному захвату и перевариванию бактерий, остатков погибших клеток и других чужеродных или токсичных для организма частиц.",
                150, 70, new Color(0.49f, 0.78f, 0.85f, 1), () =>
                    ShowMessage(
                        "Макрофаг знакомит другие клетки иммунной системы с кусочками переваренного микроба, что позволяет организму лучше бороться с инфекцией.",
                        150, 70, new Color(0.49f, 0.78f, 0.85f, 1), () =>
                            ShowMessage(
                                "Макрофаг не наносит урона угрозе, но снижает уровень ее антигена. Может показаться, что это бесполезно, но антитела дают больше шансов на победу – это станет ясно далее.",
                                150, 70, new Color(0.49f, 0.78f, 0.85f, 1), () =>
                                {
                                    dendriticCell.interactable = true;
                                    dendriticCellInfo();
                                })));
    }

    private UnityAction DendriticCellInfo()
    {
        return () => ShowMessage(
            "Дендритные клетки сходи по действию с макрофагами. Они дороже, сильнее, но медленнее.",
            150, -20, new Color(0.87f, 0.58f, 0.76f, 1), HideMessage);
    }

    private Action KillerInfo()
    {
        return () =>
        {
            tKiller.interactable = true;
            ShowMessage(
                "Т-лимфоциты – иммунные тела, которые специализируются только на одной угрозе, для которой были созданы. Поэтому для их использования необходимо сначала получить антитела к угрозе.",
                150, -230, new Color(0.19f, 0.53f, 0.47f, 1), () =>
                    ShowMessage(
                        "Т-лимфоциты так названы, потому что после образования в костном мозге дозревают в вилочковой железе — тимусе.",
                        150, -230, new Color(0.19f, 0.53f, 0.47f, 1), () =>
                            ShowMessage(
                                "Есть три подвида Т-лимфоцитов: Т-киллеры, Т-хелперы и Т-супрессоры",
                                150, -230, new Color(0.19f, 0.53f, 0.47f, 1), () =>
                                    ShowMessage(
                                        "Т-киллеры могут убивать зараженные вирусами клетки, чтобы остановить развитие инфекции.",
                                        150, -230, new Color(0.19f, 0.53f, 0.47f, 1), () =>
                                            ShowMessage(
                                                "Т-киллеры – дешевые и эффективные бойцы. Поэтому во время боя использовать нейтрофилов и НК-клетки верное тактическое решение, но получить антитела – важно стратегически. Делайте выбор в зависимости от ситуации и побеждайте!",
                                                150, -230, new Color(0.19f, 0.53f, 0.47f, 1), HideMessage)))));
        };
    }

    private IEnumerable<Action> ThreatKillingCycle(Action spawnThreat, Action info)
    {
        gameController.FirstThreatWin = false;
        yield return spawnThreat;
        yield return info;
        while (!gameController.FirstThreatWin)
            if (gameController.threats.Count == 0)
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