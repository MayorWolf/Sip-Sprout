using System;
using System.Globalization;
using TMPro;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
///     Main script that manages all user interaction and resulting changes.
/// </summary>
public class MainBehaviour : MonoBehaviour
{
    /**
     * TODO: Android default keyboard replacement
     */
    [SerializeField] private TMP_Text consumedString, streakString, notificationToggleString;

    [SerializeField] private Slider slider; //Slider displaying percentage of daily goal
    [SerializeField] private TMP_InputField customAmountInput, customGoalInput, customSleepStart, customSleepEnd;
    [SerializeField] private Sprite[] plantStatus; //List of plant sprites to display healthiness
    [SerializeField] private Image plantDisplay; //Image displaying plant using different plant sprites
    private readonly CultureInfo _culture = new("de-DE");

    private readonly string[] _notificationTitles =
        { "I am thirsty!", "Hydration Time!", "We've been trying to reach you about your extended car warranty." };

    private float _consumedWater;
    private float _consumptionGoal = 2.2f;

    private DateTime _lastDrinkTime, _lastStreakTime, _lastUsage = DateTime.Now;
    private bool _noNotifications;
    private int _sleepEnd = 8;
    private int _sleepStart = 21;
    private int _streak;

    // Start is called before the first frame update
    private void Start()
    {
        //Initialize PlayerPref En-/Decryption
        ZPlayerPrefs.Initialize("nuq99qj3ng9qp+ifemr", "834h3gnpmh-4");

        _LoadSavedData();

        _CheckForReset();

        //Update UI without changing values
        AddHydration(0);

        _UpdateStreaks();

        _NotificationSetup();
    }

    // Check if app was inactive, which prompts an Ui update and check for resets/streaks
    private void OnApplicationFocus(bool pauseStatus)
    {
        if (!pauseStatus) return;
        Debug.Log("App returned activity");
        _LoadSavedData();
        _CheckForReset();
        _UpdateStreaks();
    }

    /// <summary>
    ///     Increases the amount of water the user drank today. Accepts negative inputs, mainly for debugging/testing.
    ///     Calling this Method updates all relevant UI Elements, including Slider, Strings and the plant image
    /// </summary>
    /// <param name="addedWater">The amount of consumed water (allows negative Int)</param>
    public void AddHydration(float addedWater)
    {
        //checked for negative inputs
        _consumedWater = _consumedWater + addedWater >= 0 ? _consumedWater += addedWater : 0;

        //Update text, slider & image
        consumedString.text = "Today's Goal: " + _consumedWater.ToString("G3") + "/" + _consumptionGoal + "l";

        //Calculate slider position without dividing by 0 (because that would break the universe or something...)
        slider.value = _consumedWater == 0 ? 0 : _consumedWater / _consumptionGoal;

        _UpdateImage();

        //Save new data
        ZPlayerPrefs.SetFloat("ConsumedWater", _consumedWater);
        if (addedWater > 0) _ResetHourlyNotification();
    }

    /// <summary>
    ///     Parses Input string to float for custom Hydration values.
    ///     The Input currently uses androids default keyboard, thus allowing invalid inputs that have to be caught.
    /// </summary>
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

    /// <summary>
    ///     Allows the user to set a custom daily goal using input fields.
    ///     After setting a new goal the entire UI is updated to reflect the changes.
    ///     The Input currently uses androids default keyboard, thus allowing invalid inputs that have to be caught.
    /// </summary>
    public void SetCustomGoal()
    {
        if (!(float.Parse(customGoalInput.text) > 0)) return;
        try
        {
            _consumptionGoal = float.Parse(customGoalInput.text);

            //Save new data
            ZPlayerPrefs.SetFloat("WaterGoal", _consumptionGoal);
            //Update UI
            AddHydration(0);
        }
        catch
        {
            Debug.Log(customAmountInput.text + " is no valid input!");
        }
    }

    //Reset the consumed water of today (should be used once per day)
    private void _ResetHydration()
    {
        _consumedWater = 0;
        //Update UI
        AddHydration(0);
    }

    //Basic notification setup for Android
    private static void _NotificationSetup()
    {
        //Setup Notification channel for hourly reminder
        var channel = new AndroidNotificationChannel
        {
            Id = "SaS_regularReminders",
            Name = "Hourly Reminder",
            Importance = Importance.Default,
            Description = "Generic notifications"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        //Setup Notification channel for daily reminder
        var wakechannel = new AndroidNotificationChannel
        {
            Id = "SaS_WakeReminders",
            Name = "Daily Reminder",
            Importance = Importance.Default,
            Description = "Generic notifications"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(wakechannel);

        //Get Notification Permissions
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
    }

    //Resets notifications, delaying the next hourly notification for one hour and the next daily for the next 2 days
    private void _ResetHourlyNotification()
    {
        _UpdateStreaks();
        _lastDrinkTime = DateTime.Now;
        ZPlayerPrefs.SetString("LastDrink", _lastDrinkTime.ToString(_culture));

        //Remove scheduled notifications
        AndroidNotificationCenter.CancelAllScheduledNotifications();

        if (!_noNotifications) return;

        //Update Hourly Notification
        for (var i = 1; i <= 3; i++)
        {
            //Throw notification
            var notification = new AndroidNotification
            {
                Title = _notificationTitles[Random.Range(0, _notificationTitles.Length)],
                Text = "Remember to drink Water from time to time. You and your plants need it."
            };
            //Set notification to be thrown in one hour if thi does not fall in the custom sleep time
            if (DateTime.Now.Hour + i >= _sleepEnd && DateTime.Now.Hour + i <= _sleepStart)
            {
                notification.FireTime = DateTime.Now.AddHours(i);
                Debug.Log("Timer set for " + notification.FireTime);
            }
            //If the notification falls in to the sleep time set it for the next day
            else
            {
                notification.FireTime = _GetNextDay(_sleepEnd);
                Debug.Log("QUIET TIME! Timer set for " + notification.FireTime);
                i = 4;
            }

            //Post notification
            AndroidNotificationCenter.SendNotification(notification, "SaS_regularReminders");
        }

        //Update Daily Notification
        for (var i = 1; i <= 7; i++)
        {
            //Throw wakeup notification
            var wakeupNotif = new AndroidNotification
            {
                Title = "Did you forget something?",
                Text = "Haven't seen you in a while, might want to drink something?"
            };
            //Set notification to be thrown in one hour if thi does not fall in the custom sleep time
            if (DateTime.Now.Hour + i >= _sleepEnd && DateTime.Now.Hour + i <= _sleepStart)
            {
                wakeupNotif.FireTime = DateTime.Now.AddDays(i);
                Debug.Log("Wakeup Timer set for " + wakeupNotif.FireTime);
            }
            //If the notification falls in to the sleep time set it for the next day
            else
            {
                wakeupNotif.FireTime = _GetNextDay(_sleepEnd).AddDays(i);
                Debug.Log("QUIET TIME! Wakeup timer set for " + wakeupNotif.FireTime);
            }

            //Post notification
            AndroidNotificationCenter.SendNotification(wakeupNotif, "SaS_WakeReminders");
        }
    }

    //Checks if streak has changes and updates UI accordingly
    //Includes kitbashed failsaves for new months/years, somewhat experimental
    //ToDo: More Testing required!
    private void _UpdateStreaks()
    {
        //Continue Streak when goal has been reached
        if (
            (_lastStreakTime.Day == DateTime.Now.Day - 1 ||
             (DateTime.Now.Day == 1 && _lastStreakTime.Month == DateTime.Now.Month - 1) ||
             _lastStreakTime.Year == 1
             || _streak == 0)
            && _consumedWater >= _consumptionGoal)
        {
            Debug.Log("Streak Continued!");
            _streak++;
            _lastStreakTime = DateTime.Now;
            ZPlayerPrefs.SetString("LastStreak", DateTime.Now.ToString(_culture));
        }
        //Idle streak when the goal hasn't been reached but the day's not over
        else if (_lastDrinkTime.Day == DateTime.Now.Day ||
                 _lastStreakTime.Day == DateTime.Now.Day - 1 ||
                 (DateTime.Now.Day == 1 && _lastStreakTime.Month == DateTime.Now.Month - 1) ||
                 _lastStreakTime.Year == 1)
        {
            Debug.Log("No new Streak");
        }
        //Loose streak 
        else
        {
            Debug.Log("Streak Lost :(");
            Debug.Log(_lastStreakTime.Year);
            _streak = 0;
        }

        ZPlayerPrefs.SetInt("Streak", _streak);
        streakString.text = _streak.ToString();
    }

    /// <summary>
    ///     Toggles all notifications, effectively muting/unmuting the app
    /// </summary>
    public void ToggleNotifications()
    {
        _noNotifications = !_noNotifications;

        notificationToggleString.text = _noNotifications ? "Notifications: ON" : "Notifications: OFF";

        var boolToInt = _noNotifications ? 1 : 0;
        ZPlayerPrefs.SetInt("NoNotifications", boolToInt);
        Debug.Log("ToggleNotifications:" + _noNotifications);
    }

    //Update plant image based on curren water percentage
    private void _UpdateImage()
    {
        var temp = plantDisplay.GetComponent<Image>();
        temp.sprite = (_consumedWater / _consumptionGoal) switch
        {
            <= 0.33f => plantStatus[2],
            <= 0.66f => plantStatus[1],
            _ => plantStatus[0]
        };
    }

    //Returns the DateTime of a specific hour in the next day
    private static DateTime _GetNextDay(int hour)
    {
        var now = DateTime.Now;
        if (now.Hour >= hour) now = now.AddDays(1);
        return new DateTime(now.Year, now.Month, now.Day, hour, 0, 0);
    }

    /// <summary>
    ///     Allows the user to set a custom timespan in which they receive no notifications.
    ///     The Input currently uses androids default keyboard, thus allowing invalid inputs that have to be caught.
    /// </summary>
    public void SetCustomSleep()
    {
        try
        {
            _sleepStart = int.Parse(customSleepStart.text);
            _sleepEnd = int.Parse(customSleepEnd.text);
            ZPlayerPrefs.SetInt("SleepStart", _sleepStart);
            ZPlayerPrefs.SetInt("SleepEnd", _sleepEnd);
            //PlayerPrefs.Save();
            _ResetHourlyNotification();
        }
        catch
        {
            Debug.Log(customSleepStart.text + " is not a valid input.");
        }
    }

    //Checks if the app has not been used today
    private void _CheckForReset()
    {
        if (ZPlayerPrefs.GetString("LastUsage") != "") _lastUsage = DateTime.Parse(ZPlayerPrefs.GetString("LastUsage"));

        if (_lastUsage.Day < DateTime.Now.Day ||
            _lastUsage.Month < DateTime.Now.Month ||
            _lastUsage.Year < DateTime.Now.Year)
            _ResetHydration();
        ZPlayerPrefs.SetString("LastUsage", DateTime.Now.ToString(_culture));
    }

    //Loads all encrypted user data and decyrpt it
    private void _LoadSavedData()
    {
        //Load saved data
        _consumedWater = ZPlayerPrefs.GetFloat("ConsumedWater");
        _consumptionGoal = ZPlayerPrefs.GetFloat("WaterGoal") == 0 ? 2.2f : ZPlayerPrefs.GetFloat("WaterGoal");
        _noNotifications = ZPlayerPrefs.GetInt("NoNotifications") == 1;
        _streak = ZPlayerPrefs.GetInt("Streak");

        if (ZPlayerPrefs.GetInt("SleepStart") != 0 || ZPlayerPrefs.GetInt("SleepEnd") != 0)
        {
            _sleepStart = ZPlayerPrefs.GetInt("SleepStart");
            _sleepEnd = ZPlayerPrefs.GetInt("SleepEnd");
        }

        notificationToggleString.text =
            _noNotifications ? "Notifications: ON" : "Notifications: OFF"; //update Text accordingly

        if (ZPlayerPrefs.GetString("LastDrink") != "")
            _lastDrinkTime = DateTime.Parse(ZPlayerPrefs.GetString("LastDrink"));
        if (ZPlayerPrefs.GetString("LastStreak") != "")
            _lastStreakTime = DateTime.Parse(ZPlayerPrefs.GetString("LastStreak"));

        customGoalInput.text = _consumptionGoal.ToString(_culture);
        customSleepStart.text = _sleepStart.ToString(_culture);
        customSleepEnd.text = _sleepEnd.ToString(_culture);
    }
}