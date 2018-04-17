using System.Collections.Generic; // Possibilita o uso de listas (list)
using UnityEngine;
using System;
using Random = UnityEngine.Random; // Assegura que "Random" use o gerador de números aleatórios do Unity Engine


public class BoardManager : MonoBehaviour
{

    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [Serializable]
    public class Count
    {
        public int minimum; //Valor minimo da classe Count.
        public int maximum; //Valor maximo da classe Count.

        //Assignment constructor.
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;                                             //Número de colunas do jogo(tabuleiro).
    public int rows = 8;                                                //Número de linhas do jogo(tabuleiro).
    public Count wallcount = new Count(5, 9);                           //limites inferiores e superiores para o numero randomico de paredes por nivel.
    public Count foodcount = new Count(1, 5);                           //limites inferiores e superiores para o numero randomico de comida por nivel.
    public GameObject exit;                                             //Prefab de spawn para a saída.
    public GameObject[] floortiles;                                     //Vetor de prefabs de chao.
    public GameObject[] walltiles;                                      //Vetor de prefabs de paredes.
    public GameObject[] foodtiles;                                      //Vetor de prefabs de comida.
    public GameObject[] enemytiles;                                     //Vetor de prefabs de inimigos.
    public GameObject[] outerwalltiles;                                 //Vetor de prefabs de paredes exteriores "indestrutiveis".

    private Transform boardholder;                                      //A variable to store a reference to the transform of our Board object Variavel para alocar uma referencia para a transformada do objeto do tabuleiro.
    private List<Vector3> gridpositions = new List<Vector3>();    //A list of possible locations to place tiles.

    void initialiselist()
    {   //Clears our list gridPositions and prepares it to generate a new board.
        gridpositions.Clear();  //Clear our list gridPositions.

        for (int x = 1; x < columns - 1; x++)
        {                   //Loop through x axis (columns).
            for (int y = 1; y < rows - 1; y++)
            {               //Within each column, loop through y axis (rows).
                gridpositions.Add(new Vector3(x, y, 0f));       //At each index add a new Vector3 to our list with the x and y coordinates of that position.
            }
        }
    }

    void boardsetup()
    {                                           //Sets up the outer walls and floor (background) of the game board.
        boardholder = new GameObject("Board").transform;        //Instantiate Board and set boardHolder to its transform.

        for (int x = -1; x < columns + 1; x++)
        {               //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
            for (int y = -1; y < rows + 1; y++)
            {               //Loop along y axis, starting from -1 to place floor or outerwall tiles.
                GameObject toInstantiate = floortiles[Random.Range(0, floortiles.Length)];      //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                if (x == -1 || x == columns || y == -1 || y == rows)
                {                               //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                    toInstantiate = outerwalltiles[Random.Range(0, outerwalltiles.Length)];
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.

                instance.transform.SetParent(boardholder);      //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
            }
        }
    }

    Vector3 RandomPosition()
    {                               //RandomPosition returns a random position from our list gridPositions.
        int randomindex = Random.Range(0, gridpositions.Count);     //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
        Vector3 randomPosition = gridpositions[randomindex];            //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
        gridpositions.RemoveAt(randomindex);                            //Remove the entry at randomIndex from the list so that it can't be re-used.
        return randomPosition;                              //Return the randomly selected Vector3 position.
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {       //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
        int objectCount = Random.Range(minimum, maximum + 1);                           //Choose a random number of objects to instantiate within the minimum and maximum limits
        for (int i = 0; i < objectCount; i++)
        {                                           //Instantiate objects until the randomly chosen limit objectCount is reached
            Vector3 randomPosition = RandomPosition();                                  //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];     //Choose a random tile from tileArray and assign it to tileChoice
            Instantiate(tileChoice, randomPosition, Quaternion.identity);              //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation

        }
    }

    public void SetupScene(int level)
    {                      //SetupScene initializes our level and calls the previous functions to lay out the game board
        boardsetup();                                      //Creates the outer walls and floor.
        initialiselist();                                  //Reset our list of gridpositions.
        LayoutObjectAtRandom(walltiles, wallcount.minimum, wallcount.maximum);         //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(foodtiles, foodcount.minimum, foodcount.maximum);         //Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
        int enemyCount = (int)Mathf.Log(level, 2f);        //Determine number of enemies based on current level number, based on a logarithmic progression
        LayoutObjectAtRandom(enemytiles, enemyCount, enemyCount);                      //Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);           //Instantiate the exit tile in the upper right hand corner of our game board
    }
}