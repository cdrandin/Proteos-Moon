using UnityEngine;

[ExecuteInEditMode]

public class Create3DNoise : MonoBehaviour
{

    public Texture3D tex;
    public int size = 64;
    
    void OnEnable()
    {
        Generate();  
    }

    float fbm( float x, float y, float z)
    {
        float f=0;
        int samples = 3;
        Vector3 P = new Vector3(x, y, z);
        
        //f = 0.5000f * noise(P);
        //P= /*m * */P * 2.02f;
        //f += 0.2500f * noise(P);
        //P = /*m * */P * 2.03f;
       // f += 0.1250f * noise(P);
        for (int i = 0; i < samples; i++)
        {
            f += noise(Vector3Add(P , (float)i *0.5f));           
        }
        return f/samples;
    }

    Vector3 Vector3Add(Vector3 Vector, float Float)
    {
        return new Vector3(Vector.x + Float, Vector.y + Float, Vector.z + Float);
    }
    
    public void Generate()
    {
        
        tex = new Texture3D(size, size, size, TextureFormat.RGB24, true);


        var cols = new Color[size * size * size];
        //   float mul = 1.0f / (size-1);
        int idx = 0;
        Color c = Color.black;

        for (int z = 0; z < size; ++z)
        {
            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x, ++idx)
                {
                    c.r = fbm(x, y, z);
                    c.g = c.r;
                    c.b = c.r;

                    cols[idx] = c;
                }

            }

        }

        tex.SetPixels(cols);
        tex.Apply();
        Shader.SetGlobalTexture("_3DTex", tex);
    }
    
    //Noise https://www.shadertoy.com/view/lss3zr
    float hash(float n)
    {
        float x = Mathf.Sin(n) * 43758.5453f;
        return x - Mathf.Floor(x);

    }

    float noise(Vector3 x)
    {
        //Vector3 x= new Vector3(1f,1f,1f);
        Vector3 p = new Vector3(Mathf.Floor(x.x), Mathf.Floor(x.y), Mathf.Floor(x.z));
        Vector3 f = new Vector3(x.x - Mathf.Floor(x.x), x.y - Mathf.Floor(x.y), x.z - Mathf.Floor(x.z));

        //f = f*f*(3-2*f);
        f.x = f.x * f.x * (3f - 2f * f.x);
        f.y = f.y * f.y * (3f - 2f * f.y);
        f.z = f.z * f.z * (3f - 2f * f.z);
      
        float n = p.x + p.y * 57.0f + 113.0f * p.z;
        
        float res = Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(hash(n + 0.0f), hash(n + 1.0f), f.x),
                            Mathf.Lerp(hash(n + 57.0f), hash(n + 58.0f), f.x), f.y),
                        Mathf.Lerp(Mathf.Lerp(hash(n + 113.0f), hash(n + 114.0f), f.x),
                            Mathf.Lerp(hash(n + 170.0f), hash(n + 171.0f), f.x), f.y), f.z);
        return res;
    }

}