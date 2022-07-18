using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace multi_teszt
{
	class Program
	{
		static void Main(string[] args)
		{
			Jatekos karesz = new Jatekos("Karesz");
			Jatekos lilesz = new Jatekos("Lilesz");
			Metronom m = new Metronom();

			foreach (Jatekos jatekos in Jatekos.lista)
			{
				jatekos.thread.Start();
			}

			m.Start(20);
		}
		static void Karesz_folyamata() => Jatekos.lista[0].Folyamat();
		static void Lilesz_folyamata() => Jatekos.lista[1].Folyamat();
	}

	class Metronom
	{
		int szamlalo = 0;

		public void UjKor()
		{
			foreach (Jatekos jatekos in Jatekos.lista)
				if (!jatekos.kész)
					jatekos.Ujraindit();
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
		// instancia tulajdonságai
		public string nev;
		public Thread thread;
		private int meddig;
		public bool kész;
		public bool vár;
		// osztály tulajdonságai
		public static List<Jatekos> lista = new List<Jatekos>();
		
		public static Random r = new Random();

		// konstruktor
		public Jatekos(string nev)
		{
			this.nev = nev;
			this.thread = new Thread(new ThreadStart(Folyamat));
			this.meddig = r.Next(10,20);

			lista.Add(this);
			kész = false;

			Console.WriteLine($"{nev} létrejött, és {meddig}-ig fog elszámolni.");
		}

		// metódusok
		public void Folyamat()
		{
			for (int i = 0; i < meddig; i++)
				Lep();

			kész = true;
		}

		public void Ujraindit() 
		{
			thread.Resume();
		}
		public void Megall() 
		{
			thread.Suspend();
		}
		public void Lep()
		{
			Console.WriteLine($"{nev} lép.");
			Megall();
			vár = true;
			
		}
	}
}
