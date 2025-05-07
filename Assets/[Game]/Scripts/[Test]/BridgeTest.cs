using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class BridgeTest : MonoBehaviour
{
    [SerializeField] ChannelBridge bridge;
    [SerializeField] string userID;

    public void TestUser()
    {
        bridge.Current.Value = new BridgeData()
        {
            Key = BridgeType.INIT_USER,
            Value = new JObject()
            {
                {"user_id", userID},
                {"user_name", userID},
                {"token", userID}
            }.ToString(),
        };
    }

    public void TestChat(bool value)
    {
        bridge.Current.Value = new BridgeData()
        {
            Key = BridgeType.INIT_CHAT,
            Value = new JObject()
            {
                {"chat", value},
                {"mood_name", "MOOD"}
            }.ToString()
        };
    }

    public void TestQuizBridge()
    {
        QuizData quizData1 = new QuizData()
        {
            Content = "Answer 1",
            Order = 1,
        };
        QuizData quizData2 = new QuizData()
        {
            Content = "Answer 2",
            Order = 2,
        };
        QuizData quizData3 = new QuizData()
        {
            Content = "Answer 3",
            Order = 3,
        };
        QuizData quizData4 = new QuizData()
        {
            Content = "Answer 4",
            Order = 4,
        };
        QuizData quizData5 = new QuizData()
        {
            Content = "Answer 5",
            Order = 5,
        };
        QuizData quizData6 = new QuizData()
        {
            Content = "Answer 6",
            Order = -1,
        };
        QuizData quizData7 = new QuizData()
        {
            Content = "Answer 7",
            Order = -1,
        };
        QuizData quizData8 = new QuizData()
        {
            Content = "Answer 8",
            Order = -1,
        };
        QuizData quizData9 = new QuizData()
        {
            Content = "Answer 9",
            Order = -1,
        };

        QuizData[] data = new QuizData[] { quizData1, quizData2, quizData3, quizData4, quizData5, quizData6, quizData7, quizData8, quizData9, };

        bridge.Current.Value = new BridgeData()
        {
            Key = BridgeType.INIT_QUIZ,
            Value = JsonConvert.SerializeObject(data),
        };
    }
}