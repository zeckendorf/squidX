Texture x1;
sampler2D x1Sampler = sampler_state{texture = (x1); magfilter = NONE; minfilter = NONE; mipfilter = POINT;};
Texture x2;
sampler2D x2Sampler = sampler_state{texture = (x2); magfilter = NONE; minfilter = NONE; mipfilter = POINT;};
Texture btext;
Texture etext;
sampler2D btextSampler = sampler_state{texture = (btext); magfilter = NONE; minfilter = NONE; mipfilter = POINT;};
sampler2D etextSampler = sampler_state{texture = (etext); magfilter = NONE; minfilter = NONE; mipfilter = POINT;};

float restingVerticalDistance;
float restingHorizontalDistance;
float hNodes;
float vNodes;
float width;
float height;
float numBullets = 0;
float numExp = 0;
float invBullets;
float invExp;
bool vdown;

struct vout
{
	float4 x : POSITION;
	
            float2 t : TEXCOORD0;
       
};

vout VertexShaderFunction(float4 inPos : POSITION, float4 inTexCoords : TEXCOORD)
{
	vout o = (vout)0;
	o.x = float4(inPos.x, inPos.y, 1, 1);
	o.t = inTexCoords;
    return o;
}

float4 RelaxFunction(vout psin, float2 pixel : VPOS) : COLOR
{	float2 input;

	 input = psin.t.xy + float2(.001,.001);
	 
	float2 currentPosition = tex2D(x1Sampler, input);
    float4 relaxedPosition;										//Output; initially the current node position
    relaxedPosition.xy = currentPosition.xy;
     relaxedPosition.zw = 1;
    float2 dTemp;												//x, y difference between nodes
    float sdTemp;												//unit spring difference
    float lTemp;												//Spring length
    if (pixel.x == 0) return float4(currentPosition,1,1);
    else if (pixel.y == 0) return float4(currentPosition,1,1);
    else if (pixel.x == width-1) return float4(currentPosition,1,1);
    else if (pixel.y == height-1) return float4(currentPosition,1,1);
    else{
    
    //bullet checking
    
	for(float i = 0; i<100 && i<numBullets;i++)
		{
				
         float2 unit = tex2D(btextSampler,float2(.5,i*invBullets+.001)) - relaxedPosition;
         float l = length(unit);
         l*=l;
         if(l<.01)l=.01;
			unit = normalize(unit);
			relaxedPosition-= float4(25*unit/(l+1),0,0);
			
		}
	
	
	//end bullets
    
    
   
   /* 
    relaxedPosition = lerp(relaxedPosition,  tex2D(x1Sampler, float2(input.x, input.y+vNodes)), .05);
    
    relaxedPosition = lerp(relaxedPosition,  tex2D(x1Sampler, float2(input.x, input.y-vNodes)), .05);
    
    relaxedPosition = lerp(relaxedPosition,  tex2D(x1Sampler, float2(input.x+hNodes, input.y)), .05);
    
    relaxedPosition = lerp(relaxedPosition,  tex2D(x1Sampler, float2(input.x-hNodes, input.y)), .05);
    */
    //relax(i,j+1, i, j, restingVerticalDistance);
    dTemp = tex2D(x1Sampler, float2(input.x, input.y+vNodes)) - currentPosition;
    lTemp = sqrt(dot(dTemp,dTemp));
    sdTemp = (lTemp - restingVerticalDistance)/lTemp;
    relaxedPosition.x += sdTemp*dTemp.x*.01;
    relaxedPosition.y += sdTemp*dTemp.y*.01;
    
    //relax(i,j-1, i, j, restingVerticalDistance);
    dTemp = tex2D(x1Sampler, float2(input.x, input.y-vNodes)) - currentPosition;
    lTemp = sqrt(dot(dTemp,dTemp));
    sdTemp = (lTemp - restingVerticalDistance)/lTemp;
    relaxedPosition.x += sdTemp*dTemp.x*.01;
    relaxedPosition.y += sdTemp*dTemp.y*.01;
    
    //relax(i-1,j, i, j, restingHorizontalDistance);
    dTemp = tex2D(x1Sampler, float2(input.x-hNodes, input.y)) - currentPosition;
    lTemp = sqrt(dot(dTemp,dTemp));
    sdTemp = (lTemp - restingHorizontalDistance)/lTemp;
    relaxedPosition.x += sdTemp*dTemp.x*.01;
    relaxedPosition.y += sdTemp*dTemp.y*.01;
    
    //relax(i+1,j, i, j, restingHorizontalDistance);
	dTemp = tex2D(x1Sampler, float2(input.x+hNodes, input.y)) - currentPosition;
    lTemp = sqrt(dot(dTemp,dTemp));
    sdTemp = (lTemp - restingHorizontalDistance)/lTemp;
    relaxedPosition.x += sdTemp*dTemp.x*.01;
    relaxedPosition.y += sdTemp*dTemp.y*.01;
    
    
   
    return (relaxedPosition);}
}



float4 VerletFunction(vout psin, float2 pixel : VPOS) : COLOR
{	float2 input ;

	input = psin.t.xy + float2(.001,.001);
	
	float2 currentPosition = tex2D(x1Sampler, input);
    if (pixel.x == 0) return float4(currentPosition,1,1);
    else if (pixel.y == 0) return float4(currentPosition,1,1);
    else if (pixel.x == width-1) return float4(currentPosition,1,1);
    else if (pixel.y == height-1) return float4(currentPosition,1,1);
    else{
	float2 output = 1.993*currentPosition - .993*tex2D(x2Sampler, input);
	return float4(output, 1, 1);
	}
}


technique FluidTechnique
{
    pass Relax
    {
		 
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 RelaxFunction();
    }
    pass Verlet
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 VerletFunction();
    }
}