using Interop.UIAutomationClient;
using Interop.UIAutomationCore;

namespace TestFrameWork.WindowsAutomation.Controls
{
    public class FileDialog
    {

        public void FindAndSet(string FileName, string WindowName, string ClassName)
        {

            //WindowName = open, class name = #32770
            var automation = new CUIAutomation();
            IUIAutomationElement desktop = automation.GetRootElement();

            //Find Chrome Window
            IUIAutomationCondition nameCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, WindowName);
            IUIAutomationCondition classCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, ClassName);

            IUIAutomationCondition combinedCondition = automation.CreateAndCondition(nameCondition, classCondition);
            Thread.Sleep(2000);
            IUIAutomationElement chromeWindow = desktop.FindFirst(TreeScope.TreeScope_Descendants, combinedCondition);

            //Find FileDialogWindow
            IUIAutomationCondition fileDialogNameCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "Open");
            IUIAutomationCondition fileDialogClassCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, "#32770");

            IUIAutomationCondition fileDialogCombinedCondition = automation.CreateAndCondition(fileDialogNameCondition, fileDialogClassCondition);
            Thread.Sleep(2000);
            IUIAutomationElement fileDialog = chromeWindow.FindFirst(TreeScope.TreeScope_Descendants, fileDialogCombinedCondition);

            if (fileDialog != null)
            {
                //className = edit, automationID= 1148
                IUIAutomationCondition fileNameCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, "Edit");
                Thread.Sleep(2000);
                IUIAutomationElement fileNameBox = fileDialog.FindFirst(TreeScope.TreeScope_Descendants, fileNameCondition);
                if (fileNameBox != null)
                {
                    var valuePattern = (IUIAutomationValuePattern)fileNameBox.GetCurrentPattern(UIA_PatternIds.UIA_ValuePatternId);
                    valuePattern.SetValue(FileName);
                }

                IUIAutomationCondition openButtonNameCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, "Open");
                IUIAutomationCondition openButtonClassCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, "Button");
                IUIAutomationCondition automationIDCondition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_AutomationIdPropertyId, "1");
                IUIAutomationCondition openButtonCombinedCondition = automation.CreateAndCondition(openButtonNameCondition, automationIDCondition);

                Thread.Sleep(2000);
                IUIAutomationElement openButton = fileDialog.FindFirst(TreeScope.TreeScope_Descendants, openButtonCombinedCondition);

                openButton.SetFocus();

                if (openButton != null)
                {
                    Thread.Sleep(2000);
                    var invokePattern = (IUIAutomationInvokePattern)openButton.GetCurrentPattern(UIA_PatternIds.UIA_InvokePatternId);
                    invokePattern.Invoke();
                }
            }
        }
    }
}
