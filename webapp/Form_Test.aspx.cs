using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;

public partial class Form_Test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GlobalClass gc = new GlobalClass();
        DataTable dt = new DataTable();
        DataTableCollection dtc = null;
        StringBuilder sb = new StringBuilder();

        dtc = gc.GetTables("exec dcr_sp_form 1");
        dt = dtc[0];

        IRenderer currentRenderer;

        foreach (DataRow row in dt.Rows)
        {
            currentRenderer = MapTypeToInputRenderer(row["Type"].ToString());
            var dataFields = new Dictionary<string, string>(){
                {"TemplateFieldId", row["Template_Field_ID"].ToString()},
                {"FieldName", row["Field"].ToString()},
                {"FieldDescription", row["Template_Field_Description"].ToString()},
                {"Required", row["Required"].ToString()}
            };
            sb.AppendLine(currentRenderer.Draw(dataFields));
        }

        FormContent.Text = $"{sb}";
    }

    private IRenderer MapTypeToInputRenderer(string type)
    {
        switch (type)
        {
            case "Text 100 characters":
                return new TextInput();
            case "Text 4000 characters":
                return new TextAreaInput();
            case "Number":
            case "Money":
                return new NumberInput();
            case "Date":
                return new DateInput();
            case "Yes/No":
                return new BooleanInput();
            case "Choice (single)":
                return new SelectInput();
            default:
                return new TextInput();
        }
    }

    interface IRenderer
    {
        string Draw(Dictionary<string, string> DataFields);
    }

    public class TextInput : IRenderer
    {
        public string Draw(Dictionary<string, string> DataFields)
        {
            string isRequired = DataFields["Required"] == "1" ? "required" : null;

            return $@"<div class='mb-3'>
                        <label for='{DataFields["FieldName"]}' class='form-label d-flex flex-row align-items-baseline justify-content-between'>
                            {DataFields["FieldName"]}
                            <div class='form-text text-muted'>{DataFields["FieldDescription"]}.</div>
                        </label>
                        <input type='text' class='form-control' id='{DataFields["FieldName"]}' fid='{DataFields["TemplateFieldId"]}' {isRequired}/>
                    </div>";
        }
    }

    public class NumberInput : IRenderer
    {
        public string Draw(Dictionary<string, string> DataFields)
        {
            string isRequired = DataFields["Required"] == "1" ? "required" : null;

            return $@"<div class='mb-3'>
                        <label for='{DataFields["FieldName"]}' class='form-label d-flex flex-row align-items-baseline justify-content-between'>
                            {DataFields["FieldName"]}
                            <div class='form-text text-muted'>{DataFields["FieldDescription"]}.</div>
                        </label>
                        <input type='number' class='form-control' id='{DataFields["FieldName"]}' fid='{DataFields["TemplateFieldId"]}' {isRequired}/>
                    </div>";
        }
    }

    public class TextAreaInput : IRenderer
    {
        public string Draw(Dictionary<string, string> DataFields)
        {
            string isRequired = DataFields["Required"] == "1" ? "required" : null;

            return $@"<div class='mb-3'>
                        <label for='{DataFields["FieldName"]}' class='form-label d-flex flex-row align-items-baseline justify-content-between'>
                            {DataFields["FieldName"]}
                            <div class='form-text text-muted'>{DataFields["FieldDescription"]}.</div>
                        </label>
                        <textarea class='form-control' id='{DataFields["FieldName"]}' rows='3' {isRequired}></textarea>
                     </div>";
        }
    }

    public class BooleanInput : IRenderer
    {
        public string Draw(Dictionary<string, string> DataFields)
        {
            return $@"<div class='form-group form-check d-flex flex-row align-items-center'>
                        <input type = 'checkbox' class='form-check-input' id='{DataFields["FieldName"]}'>
                        <label class='form-check-label' for='{DataFields["FieldName"]}'>
                            <span class='d-flex flex-row align-items-baseline'>
                                {DataFields["FieldName"]}
                                &nbsp
                                <small class='form-text text-muted'>{DataFields["FieldDescription"]}.</small>
                            </span>
                        </label>
                      </div>";
        }
    }

    public class DateInput : IRenderer
    {
        public string Draw(Dictionary<string, string> DataFields)
        {
            string isRequired = DataFields["Required"] == "1" ? "required" : null;

            return $@"<div class='mb-3'>
                        <label for='{DataFields["FieldName"]}' class='form-label d-flex flex-row align-items-baseline justify-content-between'>
                            {DataFields["FieldName"]}
                            <div class='form-text text-muted'>{DataFields["FieldDescription"]}.</div>
                        </label>
                        <input type='date' class='form-control datepicker' id='{DataFields["FieldName"]}' fid='{DataFields["TemplateFieldId"]}' {isRequired}/>
                    </div>";
        }
    }

    public class SelectInput : IRenderer
    {
        public string Draw(Dictionary<string, string> DataFields)
        {
            return $@"<div class='mb-3'>
                        <label for='{DataFields["FieldName"]}' class='form-label d-flex flex-row align-items-baseline justify-content-between'>
                            {DataFields["FieldName"]}
                            <div class='form-text text-muted'>{DataFields["FieldDescription"]}.</div>
                        </label>
                        <select class='form-control' id='exampleFormControlSelect2'>
                            <option> 1 </option>
                            <option> 2 </option>
                            <option> 3 </option>
                            <option> 4 </option>
                            <option> 5 </option>
                        </select>
                     </div>";
        }
    }

};