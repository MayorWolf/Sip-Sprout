using System.Globalization;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Android;
using Unity.Notifications.Android;

public class MainBehaviour : MonoBehaviour
{
    /**
     * TODO:
     * - Test Notifications and reset
     * - Custom notification start and end time
     * - Android input comma (in better)
     */

    [SerializeField]
    private TMP_Text consumedString; //TextComponent that displays daily amount of water consumption
    [SerializeField]
    private TMP_Text notificationToggleString;//TextComponent displaying the current notification status
    [SerializeField]
    private Slider slider; //Slider displaying percentage of daily goal
    [SerializeField]
    private TMP_InputField customAmountInput; 
    [SerializeField]
    private TMP_InputField customGoalInput;
    [SerializeField]
    Sprite[] plantStatus; //List of plant sprites to display healthiness
    [SerializeField]
    private Image plantDisplay; //Image displaying plant using different plant sprites

    private readonly string[] _notificationTitles = { "I am thirsty!", "Hydration Time!", "We've been trying to reach you about your extended car warranty." };
    
    private float _consumedWater;
    private float _consumptionGoal = 2.2f;
    private bool _notificationActive = true;

    private System.DateTime _lastDrinkTime, _lastUsage = System.DateTime.Now;
    
    private readonly CultureInfo _culture = new CultureInfo("de-DE");
    // Start is called before the first frame update
    void Start()
    {
        _LoadSavedData();

        _CheckForReset();
        customGoalInput.text = _consumptionGoal.ToString(_culture);

        //Update UI without changing values
        AddHydration(0);

        _NotificationSetup();

    }
    
    // Check for app inactivity
    private void OnApplicationFocus(bool pauseStatus) {
        if(pauseStatus){
            Debug.Log("We back, we ballin");
            _CheckForReset();
        }
    }
    
    //Increase amount of water the user drank today
    public void AddHydration(float addedWater)
    {
        //checked for negative inputs
        _consumedWater = (_consumedWater + addedWater >= 0) ? _consumedWater += addedWater : 0;

        //Update text, slider & image
        consumedString.text = "Today's Goal: " + _consumedWater.ToString("G3") + "/" + _consumptionGoal + "l";
        slider.value = (_consumedWater == 0) ? 0 : (_consumedWater / _consumptionGoal); //without dividing by 0 (because that would break the universe or something...)
        _UpdateImage();

        //Save new data
        PlayerPrefs.SetFloat("ConsumedWater", _consumedWater);

        if(addedWater > 0){_ResetHourlyNotification();}

        PlayerPrefs.Save();
    }

    //Parses Input string to float for custom Hydration values
    public void AddCustomHydration()
    {
        try
        {
            AddHydration(float.Parse(customAmountInput.text));
        }
        catch
        {
            Debug.Log(customAmountInput.text + " is no valid input!");
        }
    }

    //Set a custom daily goal
    public void SetCustomGoal()
    {
        if(float.Parse(customGoalInput.text) > 0)
        {
            try{
                _consumptionGoal = float.Parse(customGoalInput.text);

                //Save new data
                PlayerPrefs.SetFloat("WaterGoal", _consumptionGoal);
                PlayerPrefs.Save();

                AddHydration(0);
            }
            catch
            {
                Debug.Log(customAmountInput.text + " is no valid input!");
            }
           
        }
    }

    //Reset )
    private void _ResetHydration()
    {
        _consumedWater = 0;
        AddHydration(0);
    }

    //Basic notification setup for Android
    private static void _NotificationSetup()
    {
        //Setup Notification channel
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        //Get Notification Permissions
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }        
    }

    private void _ResetHourlyNotification()
    {
        _lastDrinkTime = System.DateTime.Now;
        PlayerPrefs.SetString("LastDrink", _lastDrinkTime.ToString(_culture));
        PlayerPrefs.Save();

        //Remove scheduled notifications
        AndroidNotificationCenter.CancelAllScheduledNotifications();

        if (_notificationActive)
        {
            //Throw notification
            var notification = new AndroidNotification
            {
                Title = _notificationTitles[Random.Range(0, _notificationTitles.Length)],
                Text = "Remember to drink Water from time to time. You and your plants need it."
            };
            if (System.DateTime.Now.Hour + 1 >= 8 && System.DateTime.Now.Hour + 1 <= 20)
            {
                notification.FireTime = System.DateTime.Now.AddHours(1);
                Debug.Log("Timer set for " + notification.FireTime);
            }
            else
            {
                notification.FireTime = _GetNext8AM();
                Debug.Log("QUIET TIME! Timer set for " + notification.FireTime);
            }

            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
    }

    public void ToggleNotifications()
    {
        _notificationActive = !_notificationActive;

        notificationToggleString.text = (_notificationActive) ? "Notifications: ON" : "Notifications: OFF";

        int boolToInt = (_notificationActive) ? 1 : 0;
        PlayerPrefs.SetInt("NotificationsActive", boolToInt);
        Debug.Log("ToggleNotifications:" + _notificationActive);
        PlayerPrefs.Save();
    }

    //Update plant image based on curren water percentage
    private void _UpdateImage()
    {
        var temp = plantDisplay.GetComponent<Image>();
        if (_consumedWater / _consumptionGoal <= 0.33f)
        {
            temp.sprite = plantStatus[2];
        }
        else if (_consumedWater / _consumptionGoal <= 0.66f)
        {
            temp.sprite = plantStatus[1];
        }
        else
        {
            temp.sprite = plantStatus[0];
        }
    }

    // TODO: Make flexible, not just 8 am
    private static System.DateTime _GetNext8AM()
    {
        System.DateTime now = System.DateTime.Now;
        if (now.Hour >= 8)
        {
            now = now.AddDays(1);
        }
        return new System.DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
    }

    private void _CheckForReset()
    {
            if (PlayerPrefs.GetString("LastUsage") != "")
            {
                _lastUsage = System.DateTime.Parse(PlayerPrefs.GetString("LastUsage"));
            }
        

        if (_lastUsage.Day < System.DateTime.Now.Day)
        {
            _ResetHydration();
        }
        PlayerPrefs.SetString("LastUsage", System.DateTime.Now.ToString(_culture));
        PlayerPrefs.Save();
    }

    private void _LoadSavedData()
    {
        //Load saved data
        _consumedWater = PlayerPrefs.GetFloat("ConsumedWater");
        _consumptionGoal = (PlayerPrefs.GetFloat("WaterGoal") == 0) ? 2.2f : PlayerPrefs.GetFloat("WaterGoal");
        _notificationActive = (PlayerPrefs.GetInt("NotificationsActive") == 1);
        notificationToggleString.text = (_notificationActive) ? "Notifications: ON" : "Notifications: OFF"; //update Text accordingly

       if (PlayerPrefs.GetString("LastDrink") != "") { _lastDrinkTime = System.DateTime.Parse(PlayerPrefs.GetString("LastDrink")); }
    }
}
