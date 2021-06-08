using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlotController : MonoBehaviour
{
    public Canvas canvas;
    public GameController gameController;
    public GameObject messagePrefab;
    private GameObject messageObject;

    public IEnumerable<Action> PlotActions()
    {
        yield return InnateImmunityInfo(LymphnodeInfo(ProteinInfo()));
        yield return () =>
            gameController.SpawnThreat(new Vector2(2, -2), new ThreatData(0, "Первая ранка", ThreatType.Wound), 1);
        yield return WoundInfo(NeutrophilInfo());
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
                -130, -110, new Color(0.98f, 0.86f, 0.69f, 1), () =>
                    ShowMessage(
                        "Все раны (кроме операционных) первично загрязнены микробами. Однако развитие инфекционного процесса в организме наблюдается не при всех ранениях.",
                        -130, -110, new Color(0.98f, 0.86f, 0.69f, 1), () =>
                            ShowMessage(
                                "Классификация раневых инфекций: инфекция раны, околораневой абсцесс, раневая флегмона, гнойный затек, свищ, тромбофлебит, лимфангит и лимфаденит.",
                                -130, -110, new Color(0.98f, 0.86f, 0.69f, 1), neutrophilInfo)));
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
}