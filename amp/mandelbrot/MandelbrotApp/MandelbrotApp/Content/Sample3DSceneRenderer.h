﻿#pragma once

#include "..\DeviceResources.h"
#include "ShaderStructures.h"
#include "..\Common\StepTimer.h"

namespace MandelbrotApp
{
	struct ModelViewProjectionConstantBuffer
	{
		DirectX::XMFLOAT4X4 model;
		DirectX::XMFLOAT4X4 view;
		DirectX::XMFLOAT4X4 projection;
	};

	// This sample renderer instantiates a basic rendering pipeline.
	class Sample3DSceneRenderer
	{
	public:
		Sample3DSceneRenderer(const std::shared_ptr<DeviceResources>& deviceResources);
		void CreateDeviceDependentResources();
		void CreateWindowSizeDependentResources();
		void ReleaseDeviceDependentResources();
		void Update(DX::StepTimer const& timer);
		void Render();

        void PointerPressed(Windows::Foundation::Point const & p);
        void PointerMoved(Windows::Foundation::Point const & p);

	private:
		// Cached pointer to device resources.
		std::shared_ptr<DeviceResources> m_deviceResources;

        std::shared_ptr<Concurrency::accelerator_view> m_av;

		// Direct3D resources for cube geometry.
		Microsoft::WRL::ComPtr<ID3D11InputLayout>	m_inputLayout;
		Microsoft::WRL::ComPtr<ID3D11Buffer>		m_vertexBuffer;
		Microsoft::WRL::ComPtr<ID3D11Buffer>		m_indexBuffer;
		Microsoft::WRL::ComPtr<ID3D11VertexShader>	m_vertexShader;
		Microsoft::WRL::ComPtr<ID3D11PixelShader>	m_pixelShader;
		Microsoft::WRL::ComPtr<ID3D11Buffer>		m_constantBuffer;

        Microsoft::WRL::ComPtr<ID3D11Texture2D>             m_mandelBrotTexture     ;
        Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_mandelBrotTextureView ;
        Microsoft::WRL::ComPtr<ID3D11SamplerState>          m_mandelBrotSampler     ;

        Microsoft::WRL::ComPtr<ID3D11Texture2D>             m_juliaTexture          ;
        Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_juliaTextureView      ;

		// System resources for cube geometry.
		ModelViewProjectionConstantBuffer m_constantBufferData;
		uint32	m_indexCount;

		// Variables used with the rendering loop.
		bool	m_loadingComplete;
		float	m_degreesPerSecond;

        Windows::Foundation::Point m_currentPoint;
        Windows::Foundation::Size  m_currentBounds;
	};
}

