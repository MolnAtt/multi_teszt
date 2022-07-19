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
			Robot defaultkaresz = new Robot("DefaultKaresz");
			Robot karesz = new Robot("Karesz");
			Robot lilesz = new Robot("Lilesz");

			Robot.játék_elindítása();
		}
	}



	class Robot
	{
		// instancia tulajdonságai
		public string nev;
		public Thread thread;
		private int meddig;

		public Robot rákövetkezője, megelőzője;
		public bool kész;
		public bool vár;

		// string-reprezentáció debughoz
		public override string ToString() => this.nev;

		// osztály tulajdonságai
		public static Random r = new Random();
		private static Robot kezdő_robot;

		// konstruktor
		public Robot(string nev)
		{
			this.nev = nev;
			this.thread = new Thread(new ThreadStart(FELADAT_BUROK));
			this.meddig = r.Next(10,20);
			this.kész = false;
			this.vár = false;

			if (kezdő_robot == null)
				kezdő_robot = this;

			Végére_fűz();

			Console.WriteLine($"{nev} létrejött, és {meddig}-ig fog elszámolni.");
		}

		#region A lánc adatszerkezet metódusai

		public void Beszúr_ez_elé(Robot ez)
		{
			this.rákövetkezője = ez;
			this.megelőzője = ez.megelőzője;
			this.rákövetkezője.megelőzője = this;
			this.megelőzője.rákövetkezője = this;
		}
		/// <summary>
		/// ha már csak egyelemű a lista, akkor hatástalan.
		/// </summary>
		public void Kifűz()
		{
			this.rákövetkezője.megelőzője = this.megelőzője;
			this.megelőzője.rákövetkezője = this.rákövetkezője;
		}
		public void Végére_fűz() => Beszúr_ez_elé(kezdő_robot);

		#endregion

		#region A robot kezelésének metódusai
		/// <summary>
		/// ez az, amit futtat majd a thread
		/// </summary>
		void FELADAT_BUROK()
		{
			FELADAT();
			kész = true;
		}
		/// <summary>
		/// ez az, amit a user szerkeszt
		/// </summary>
		void FELADAT()
		{
			for (int i = 0; i < meddig; i++)
				Csinál_valamit(); // ide jönnek az olyan parancsok, mint a tegyél le egy követ meg a lépj...
		}
		void Csinál_valamit()
		{
			Console.WriteLine($"{nev} lép.");
			Letelt_a_köröd();
		}

		#endregion

		#region A láncba fűzött szálak indítgatásának metódusai
		/// <summary>
		/// a játékosok láncán elkezd végighaladni a léptetés. 
		/// Aki kész van, kiesik a láncból. 
		/// Addig megy, míg mindenki kész nem lesz. 
		/// </summary>
		public static void játék_elindítása()
		{
			kezdő_robot.Te_jössz();
			Várakozik_amig_mindenki_kesz_nem_lesz();
			Console.WriteLine("Vége, mindenki lelépett mindent.");
		}

		static void Várakozik_amig_mindenki_kesz_nem_lesz()
		{
			while (Valaki_még_dolgozik())
				Thread.Sleep(1000);
		}

		static bool Valaki_még_dolgozik()
		{
			if (!kezdő_robot.kész)
			{
				Console.WriteLine($"{kezdő_robot} még dolgozik");
				return true;
			}
			Robot aktuális_robot = kezdő_robot.rákövetkezője;
			while (aktuális_robot != kezdő_robot)
			{
				if (!aktuális_robot.kész)
				{
					Console.WriteLine($"{aktuális_robot} még dolgozik");
					return true;
				}
				aktuális_robot = aktuális_robot.rákövetkezője;
			}
			Console.WriteLine("Mindenki készen van.");
			return false;
		}

		void Te_jössz()
		{
			if (!kész)
				Start_or_Resume();
		}
		void Letelt_a_köröd() 
		{
			thread.Suspend();
			vár = true;
			rákövetkezője.Te_jössz();
		}
		void Start_or_Resume()
		{
			if (vár)
				thread.Resume();
			else
				thread.Start();
		}

		#endregion

		// metódusok


	}
}
