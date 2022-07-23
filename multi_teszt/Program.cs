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

			Console.WriteLine("-------------------------------------------------------");
			Console.WriteLine($"JÁTÉKMESTER: elindítom a játékot");

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

		static readonly int rmin = 10;
		static readonly int rmax = 20;
		static readonly int timeout = 40;
		static int counter = 0;

		// konstruktor
		public Robot(string nev)
		{
			this.nev = nev;
			this.thread = new Thread(new ThreadStart(FELADAT_BUROK));
			this.meddig = r.Next(rmin,rmax);
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
			Console.WriteLine($"{nev}: KÉSZEN VAGYOK! Lépek.");
		}
		/// <summary>
		/// ez az, amit a user szerkeszt
		/// </summary>
		void FELADAT()
		{
			for (int i = 0; i < meddig; i++)
			{
				Csinál_valamit(i); // ide jönnek az olyan parancsok, mint a tegyél le egy követ meg a lépj...
			}
		}
		void Csinál_valamit(int i)
		{
			Console.WriteLine($"=============== SZÜNET ================= {nev} körében");
			Thread.Sleep(1000);
			Console.WriteLine($"{nev} lép. ({i}/{meddig})");
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
			counter = 0;
			kezdő_robot.Te_jössz();
			Várakozik_amig_mindenki_kesz_nem_lesz();
			Console.WriteLine("JÁTÉKMESTER: Vége, mindenki lelépett mindent, vagy lejárt az idő.");
		}

		static void Várakozik_amig_mindenki_kesz_nem_lesz()
		{
			while (Valaki_még_dolgozik() && counter<timeout)
			{
				Console.WriteLine($"JÁTÉKMESTER: körbenézek, mi van. ({++counter})");
				Thread.Sleep(1000);
			}
			if (counter < timeout)
			{
				Console.WriteLine($"JÁTÉKMESTER: elérte a counter ({counter}) a timeout-ot ({timeout})");
			}
			else
			{
				Console.WriteLine($"JÁTÉKMESTER: már nem dolgozik senki");
			}
		}

		static bool Valaki_még_dolgozik()
		{
			if (!kezdő_robot.kész)
			{
				Console.WriteLine($"JÁTÉKMESTER: {kezdő_robot} még dolgozik");
				return true;
			}
			Robot aktuális_robot = kezdő_robot.rákövetkezője;
			while (aktuális_robot != kezdő_robot)
			{
				if (!aktuális_robot.kész)
				{
					Console.WriteLine($"JÁTÉKMESTER: {aktuális_robot} még dolgozik");
					return true;
				}
				aktuális_robot = aktuális_robot.rákövetkezője;
			}
			Console.WriteLine("Mindenki készen van.");
			return false;
		}

		void Te_jössz()
		{
			Console.WriteLine($"{nev}: én jövök.");

			if (!kész)
			{
				// Console.WriteLine($"{nev}: nem vagyok kész, szóval indítok.");
				Start_or_Resume();
			}
		}
		void Letelt_a_köröd() 
		{
			vár = true;
			Console.WriteLine($"{nev}: várakozom, mindjárt felfüggesztem magam, de előbb szólok {rákövetkezője}-nek, hogy ő jön.");
			rákövetkezője.Te_jössz();
			Console.WriteLine($"{nev}: és most felfüggesztem magam");
			thread.Suspend();
		}
		void Start_or_Resume()
		{
			if (vár)
			{
			//	Console.WriteLine($"{nev}: vártam, szóval folytatom (resume).");
				thread.Resume();
			}
			else
			{
			//	Console.WriteLine($"{nev}: nem vártam, szóval startolok.");
				thread.Start();
			}
		}

		#endregion

		// metódusok


	}
}
