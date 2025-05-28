using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Godot;



public partial class NotificationHandler: Node
{
    public static NotificationHandler Instance { get; set; }
        
    private readonly string SuccessControlPath = @"res://Scenes/Warnings/Done.tscn";
    private Control SuccessControl { get; set; }
    private readonly string WarningControlPath = @"res://Scenes/Warnings/Warning.tscn";
    private Control WarningControl { get; set; }
    private readonly string FailureControlPath = @"res://Scenes/Warnings/Error.tscn";
    private Control FailureControl { get; set; }

    private Queue<NotificationModel> NotificationsQueue = new Queue<NotificationModel>();
    private bool IsQueueClearing = false;
    public bool isQueueClearing 
    {
        get
        {
            return IsQueueClearing;
        }
    }

    private bool Loaded { get; set; }

    private readonly int NotificationTime = 9;

    private Dictionary<NotificationType, Control> ControlsDict { get; set; }

    private List<Control> GetAllControls { get
        {
            return new List<Control>() { SuccessControl, WarningControl, FailureControl};
        } }

    public override void _Ready()
    {
        if(Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}
		Instance = this;

        PackedScene packedSuccess = GD.Load<PackedScene>(SuccessControlPath);
        SuccessControl = packedSuccess.Instantiate<Control>();

        PackedScene packedWarning = GD.Load<PackedScene>(WarningControlPath);
        WarningControl = packedWarning.Instantiate<Control>();

        PackedScene packedFailure = GD.Load<PackedScene>(FailureControlPath);
        FailureControl = packedFailure.Instantiate<Control>();

        ControlsDict = new Dictionary<NotificationType, Control>()
        {
            { NotificationType.Success, SuccessControl },
            { NotificationType.Warning, WarningControl },
            { NotificationType.Failure, FailureControl }
        };

        GetAllControls.ForEach(control =>
        {
            control.GetNode<Button>("HBoxContainer/ExitContainer/Button").Pressed += () => control.Visible = false;    
            control.Visible = false;
        }
        );

        EventHandler.Instance.AddListener("failureNotification", param => PushNotification(NotificationType.Failure, param as string));
        EventHandler.Instance.AddListener("successNotification", param => PushNotification(NotificationType.Success, param as string));
        EventHandler.Instance.AddListener("warningNotification", param => PushNotification(NotificationType.Warning, param as string));

    }

    public async void ClearNotificationQueue()
    {
        if(IsQueueClearing) return;
        IsQueueClearing = true;
        GD.Print($"Method has been called");
        while(NotificationsQueue.Count > 0)
        {
            NotificationModel model = NotificationsQueue.Dequeue();
            GD.Print($"Clearing... \nText = {model.text}");
            await ShowNotification(model.type, model.text);
            GD.Print($"Cleared!");
        }
        GD.Print($"Everything is cleared");
        IsQueueClearing = false;
    }

    private async Task ShowNotification(NotificationType type, string text)
    {
        if(Loaded == false)
        {
            GetAllControls.ForEach(el => Controller.Instance.AddChild(el));
            Loaded = true;
        }

        var control = ControlsDict[type];
        var label =  control.GetNode<Label>("HBoxContainer/TextContainer/Label");

        label.Text = text;
        control.Visible = true;

        await ToSignal(GetTree().CreateTimer(NotificationTime), "timeout");

        if (control.Visible) control.Visible = false;
    }

    public void PushNotification(NotificationType type, string text)
    {
        NotificationsQueue.Enqueue(new NotificationModel() { type = type, text = text});
        ClearNotificationQueue();
    }

}
public enum NotificationType
{
    Success, Warning, Failure
}

public class NotificationModel
{
    public NotificationType type { get; set; }
    public string text { get; set; }
}