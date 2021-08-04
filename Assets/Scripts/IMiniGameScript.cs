using Newtonsoft.Json.Linq;

public interface IMiniGameScript
{
    public void ReceivePlayerData(int fromDeviceID, JToken data);
    public void SetUIActive();
    public void SetPlayers(int curChallenger, int curDefender);
}
