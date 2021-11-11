Shader "Hidden/PostProcessing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		_Color("Color", Color) = (1,1,1,1)
		_Strength("Effect Strength", float) = 0

		_EffectTexture("EffectTexture", 2D) = "white" {}

		_Exponent("Exponent", float)=10

		_ColorSampling("ColorSampling", float) = 8
		_PixelSampling("PixelSampling", float) = 8


    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			//Helper function for calculating vignettes
float Vignette(float2 uv)
{
			//Becomes big in the middle small outside
			float vignette = uv.x * uv.y * (1 - uv.x) * (1 - uv.y);
			//makes sure value is between 0 and 1 (biggest value was 1/16)
			return clamp(16.0 * vignette, 0, 1);
		}

            struct appdata
            {
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
	};

            struct v2f
            {
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.uv2 = v.uv2;
                return o;
            }

			sampler2D _MainTex;
			sampler2D _EffectTexture;
			fixed4 _Color;
			float _Strength;
			float _Exponent;
			float _ColorSampling;
			float _PixelSampling;



			fixed4 frag(v2f i) : SV_Target
			{
			float totalResolution = 1024;
			float downSample = _PixelSampling;
			float offset = downSample / 2;

			float2 uv = i.uv;

			 uv = uv - ((uv * totalResolution) % downSample - offset) / totalResolution;


			float4 col = tex2D(_MainTex, uv);


			//return col;


			float colorResolution = _ColorSampling;




			//float4 newCol = (col * 256 % colorResolution) / (256 / colorResolution);

			//return newCol;

			float4 newCol = col - ((col * 256) % colorResolution - offset) / 256;

			return newCol;

			}




            ENDCG
        }
    }
}

/*

// Invert
fixed4 frag(v2f i) : SV_Target
{
	fixed4 col = tex2D(_MainTex, i.uv);
col.rgb = 1 - col.rgb;
return col;
}


*/


/*

			// Change Color
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
			col.rgb = col.rgb*_Color.rgb;


			//col.rgb = _Strength*col.rgb*_Color.rgb + (1-_Strength)*col.rgb;
			//col.rbg = lerp(col.rgb, col.rgb*_Color.rgb, _Strength);

			//col.rgb = col.rgb+_Color.rgb*_Strength;
			return col;
			}


*/


/*

			// Invert less extreme
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb = _Strength*(1 - col.rgb) + (1- _Strength)*col.rgb;
				return col;
			}


*/

/*

			// Puts a texture on top
			fixed4 frag(v2f i) : SV_Target
			{

			fixed4 col = tex2D(_MainTex, i.uv);
			fixed4 col2 = tex2D(_EffectTexture, i.uv2);


			//return (1 - _Strength)*col + _Strength * (col2+col);


			return (1 - _Strength)*col + _Strength * col2*col;

			}
*/



/*
			// Mirror
			fixed4 frag (v2f i) : SV_Target
			{

				float2 uv = float2(1 - i.uv.x, i.uv.y);

				//float2 uv = float2(i.uv.x, 1-i.uv.y);


				fixed4 col = tex2D(_MainTex, uv);
			return col;
			}

*/


/*

			// Mirror at certain screen space
			fixed4 frag (v2f i) : SV_Target
			{

			fixed4 col = tex2D(_MainTex, i.uv);


			float2 uv = float2(1 - i.uv.x, i.uv.y);

			//float2 uv = float2(i.uv.x, 1-i.uv.y);
			fixed4 col2 = tex2D(_MainTex, uv);

			fixed val = tex2D(_EffectTexture, i.uv2).a;



			return (1-val)*col + val*col2;
			}



*/


/*
// Vignette (braucht kreistextur)
			fixed4 col = tex2D(_MainTex, i.uv);
			fixed4 col2 = tex2D(_EffectTexture, i.uv2);

			//col2 = fixed4(1-col2.a, 1-col2.a, 1-col2.a, 1);

			return (col2.a)*col + (1-col2.a) * _Color;

*/

/*

			// Make the corners change color
			fixed4 frag(v2f i) : SV_Target
			{
			fixed4 col = tex2D(_MainTex, i.uv);

			float right = i.uv.x;
			float left = 1 - i.uv.x;
			float top = i.uv.y;
			float bot = 1 - i.uv.y;

				right = pow(right, _Exponent);
				left = pow(left, _Exponent);
				top = pow(top, _Exponent);
				bot = pow(bot, _Exponent);

				float border = right + left + top + bot;

				col = col + _Strength * _Color*(border);

				//col = (1- border) * col + _Strength * _Color*(border)*col;


			return col;
			}


*/


/*

			// Mix some pixels
			fixed4 frag(v2f i) : SV_Target
			{

			float4 col = tex2D(_MainTex, i.uv);

			float distance = 0.002;

			float2 left = float2(i.uv.x - distance,i.uv.y);
			float2 right = float2(i.uv.x + distance, i.uv.y);
			float2 bot = float2(i.uv.x, i.uv.y - distance);
			float2 top = float2(i.uv.x, i.uv.y + distance);


			float4 colLeft = tex2D(_MainTex, left);
			float4 colRight = tex2D(_MainTex, right);
			float4 colBot = tex2D(_MainTex, bot);
			float4 colTop = tex2D(_MainTex, top);

			float4 colMix = (colLeft + colRight + colBot + colTop) / 4;

			float4 colResult = (col + _Strength * colMix) / (1 + _Strength);


			return colResult;

			}


*/


/*

			// Color Simplification
			fixed4 frag(v2f i) : SV_Target
			{

			float colorResolution = _ColorSampling;


			float4 col = tex2D(_MainTex, i.uv);


			//float4 newCol = (col * 256 % 16) / (256 / 16);

			//return newCol;

			float offset = 0;

			float4 newCol = col-((col * 256) % colorResolution - offset)/ 256;

			return newCol;

			}




*/


/*

			// Pixelation
			fixed4 frag(v2f i) : SV_Target
			{


			float totalResolution = 1024;
			float downSample = _PixelSampling;
			float offset = downSample/2;

			float2 uv = i.uv;

			 uv = uv - ((uv * totalResolution ) % downSample - offset) / totalResolution;


			float4 col = tex2D(_MainTex, uv);


			//return col;


			float colorResolution = _ColorSampling;




			//float4 newCol = (col * 256 % colorResolution) / (256 / colorResolution);

			//return newCol;

			float4 newCol = col-((col * 256) % colorResolution - offset)/ 256;

			return newCol;

			}

*/


/*

// Needs blood splatter texture
// Effect texture
				fixed val = tex2D(_EffectTexture, i.uv2).a;

				//Normal texture
				fixed4 screenColor = tex2D(_MainTex, i.uv);
				//*

				//float right = i.uv.x;
				//float left = 1 - i.uv.x;
				//float top = i.uv.y;
				//float bot = 1 - i.uv.y;

				//right = pow(right, _Exponent);
				//left = pow(left, _Exponent);
				//top = pow(top, _Exponent);
				//bot = pow(bot, _Exponent);

				//float border = right + left + top + bot;

				//float alpha = val * border;


				//float alpha = val* border;
				

float vignette = i.uv2.x * i.uv2.y * (1 - i.uv2.x) * (1 - i.uv2.y);
vignette = clamp(16.0 * vignette, 0, 1);
vignette = 1 - vignette;


float alpha = val * vignette;

if (alpha < 1 - _Strength)
{
	alpha = 0;
}
else {
	alpha = alpha * _Strength;
}

return fixed4(((1 - alpha)*screenColor.rgb + alpha * _Color.rgb), screenColor.a);



*/


/*
// BLOOD EFFECT BETTER (Needs texture)

				//Effect texture at that position
				float overlay = tex2D(_EffectTexture, i.uv2).a;
				overlay = (overlay * _Strength);


				float vignette = Vignette(i.uv);
				// overlay is at max 1 so vignette becomes a lot bigger the smaller overlay is.
				// while still staying 0 where it was 0 (So it only is 0 or small where there is an overlay,
				// and where it is far away from the center
				vignette = (vignette / overlay);

				//reversing it (so the values near the edges which have texture become big the others 0)
				//And saturate to make sure the values are between 0 and 1
				vignette = 1 - saturate(vignette);

				//Main color of the screen
				float4 screenColor = tex2D(_MainTex, i.uv);

				// making it the chosen color
				float alpha = vignette * _Color.a * _Strength;

				//mixing the two colors
				return float4(lerp(screenColor.rgb, _Color.rgb, alpha), screenColor.a);


*/


/*

				// AUFGABE 1 Color ändern ohne Helligkeit ändern
			float4 col = tex2D(_MainTex, i.uv);

			float brightness = col.r + col.g + col.b;

			col.rgb = col.rgb*_Color.rgb;

			float newBrightness= col.r + col.g + col.b;

			col.rgb = col.rgb / newBrightness * brightness;


			// auskommentieren wenn ihr output von Aufgabe 2 möchtet.
			return col;

			// Aufgabe 2: Streifen machen im Bild.

			float totalResolution = 1024;
			float downSample = _PixelSampling;

			float2 uv = i.uv;

			float2 brightAddition = ((uv * totalResolution) % downSample)/ downSample;


			//uv = uv - ((uv * totalResolution) % downSample - offset) / totalResolution;


			col = tex2D(_MainTex, uv);

			//return fixed4(brightAddition.x, brightAddition.x, brightAddition.x, brightAddition.x)*totalResolution;

			return (1- _Strength)*col + brightAddition.x*_Color*_Strength;


*/