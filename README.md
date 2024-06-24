# About
The transition from build-in to SRP renderers is implemented by Unity developers. The transition of a long-standing project from one SRP to another can be painful. There are transition solutions in the Asset Store, but not all the details are taken into account.

If you need to create masks 🖼️ for hdrp materials (1 texture with four channels: metallic, occlusion, details, smoothness) or transfer materials 🌏 from one Lit shader to another. you can use this converter.

⚠️ Currently, conversion functions are implemented only from URP to HDRP.

# Usage
🖱️ Open **Tools > SRP Converter** in menu to show converter window.
- 🖼️ Use [Texture Mask Converter Window](https://github.com/ValeryPopov1995/srp-converter/blob/main/Plugins/SRPConverter/Editor/TextureMaskConverterWindow.cs) to convert textures and masks.
- 🌏 Use [SRP Material Converter](https://github.com/ValeryPopov1995/srp-converter/blob/main/Plugins/SRPConverter/Editor/SRPMaterialConverterWindow.cs) Window to convert materials and auto-insert the desired textures.
- 🗒️ You can use static [Texture Mask Converter](https://github.com/ValeryPopov1995/srp-converter/blob/main/Plugins/SRPConverter/Editor/TextureMaskConverter.cs) to convert textures manually or in your script logic.

⏳ [Progress Window](https://github.com/ValeryPopov1995/srp-converter/blob/main/Plugins/SRPConverter/Editor/ProgressWindow.cs) is auxiliary and allows you to monitor the conversion process.

# Texture Mask Converter Window
![Скриншот 24-06-2024 182856](https://github.com/ValeryPopov1995/srp-converter/assets/72905449/f108db9a-b1c3-4538-bc4f-1444cb635f9c)
![Скриншот 24-06-2024 183202](https://github.com/ValeryPopov1995/srp-converter/assets/72905449/8a215576-8de3-4c43-9fc3-fcefe39f4130)
# SRP Material Converter
![Скриншот 24-06-2024 183251](https://github.com/ValeryPopov1995/srp-converter/assets/72905449/3c888d30-21bc-4837-8d8b-bfcbe102ec28)
![Скриншот 24-06-2024 183350](https://github.com/ValeryPopov1995/srp-converter/assets/72905449/e6e3af94-05aa-40ad-9330-0afd2ea1cb38)
