using PassengerProfile.Reports;
using PassengerProfile.Repository;
using PassengerProfile.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PassengerProfile.ReportViewers
{
    public partial class PassengerRptViewer : Form
    {
        List<PassengerViewModel> rptList = new List<PassengerViewModel>();
        PassengerRepo repo = new PassengerRepo();

        public PassengerRptViewer(List<PassengerViewModel> list)
        {
            InitializeComponent();
            rptList = list;
        }

        public PassengerRptViewer()
        {
            InitializeComponent();
        }

        private void PassengerRptViewer_Load(object sender, EventArgs e)
        {
            PassengerReport rpt = new PassengerReport();
            rpt.SetDataSource(rptList);
            var facilityData = repo.GetAllFacilities();
            rpt.Subreports["FacilitiesReport.rpt"].SetDataSource(facilityData);
            crystalReportViewer1.ReportSource = rpt;
            crystalReportViewer1.Refresh();
        }

       
    }
}
