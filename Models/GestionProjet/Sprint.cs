namespace Thiskord_Back.Models.GestionProjet
{
    public class Sprint
    {
        public int sprint_id { get; set; }
        public string sprint_goal { get; set; }
        public string sprint_begin_date { get; set; }
        public string sprint_end_date { get; set; }

        public Sprint(int _sprint_id, string _sprint_goal, string _sprint_begin_date, string _sprint_end_date)
        {
            this.sprint_id = _sprint_id;
            this.sprint_goal = _sprint_goal;
            this.sprint_begin_date = _sprint_begin_date;
            this.sprint_end_date = _sprint_end_date;
        }
    }
}
