using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multi_teszt
{
	class Program
	{
		static void Main(string[] args)
		{
			Jatekos karesz = new Jatekos("Karesz");
			Jatekos lilesz = new Jatekos("Lilesz");
			Metronom m = new Metronom();

			karesz.Folyamat(10);
			lilesz.Folyamat(15);
			m.Start(20);
		}
	}

	class Metronom
	{
		int szamlalo = 0;

		public void UjKor()
		{
			foreach (Jatekos jatekos in Jatekos.lista)
				jatekos.Indit();
			Console.WriteLine($"{szamlalo++}. kör vége");
		}

		public void Start(int meddig)
		{
			for (int i = 0; i < meddig; i++)
				UjKor();
		}
	}

	class Jatekos
	{
		public string nev;
		
		public static List<Jatekos> lista = new List<Jatekos>();

		public Jatekos(string nev)
		{
			this.nev = nev;
			lista.Add(this);
		}

		public void Folyamat(int meddig)
		{
			for (int i = 0; i < meddig; i++)
				Lep();
		}

		public void Indit() 
		{ 
			
		}
		public void Megall() 
		{ 

		}
		public void Lep()
		{
			Console.WriteLine($"{nev} lép.");
			Megall();
		}
	}
}
