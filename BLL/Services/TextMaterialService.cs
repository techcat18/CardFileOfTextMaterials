﻿using AutoMapper;
using BLL.Validation;
using Core.DTOs;
using Core.Models;
using Core.RequestFeatures;
using DAL.Abstractions.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class TextMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public TextMaterialService(IUnitOfWork unitOfWork, 
            IMapper mapper,
            UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<TextMaterialDTO>> GetTextMaterials(TextMaterialParameters parameters)
        {
            var textMaterials = await _unitOfWork.TextMaterialRepository.GetWithDetailsAsync(parameters);

            return PagedList<TextMaterialDTO>.ToPagedList(_mapper.Map<IEnumerable<TextMaterialDTO>>(textMaterials),parameters.PageNumber,parameters.PageSize);
        }

        public async Task<IEnumerable<TextMaterialDTO>> GetTextMaterialsByCategory(TextMaterialCategoryDTO categoryDTO)
        {
            if (categoryDTO == null)
            {
                throw new CardFileException("categoryDTO was null");
            }

            var category = await _unitOfWork.TextMaterialCategoryRepository.GetByTitleAsync(categoryDTO.Title);

            if (category == null)
            {
                throw new CardFileException($"Category with title {categoryDTO.Title} doesn't exist");
            }

            var textMaterials = await _unitOfWork.TextMaterialRepository.GetByCategory(category);

            if (textMaterials == null)
            {
                throw new CardFileException($"No text materials of category {category.Title} were found");
            }

            return _mapper.Map<IEnumerable<TextMaterialDTO>>(textMaterials);
        }

        public async Task<TextMaterialDTO> GetTextMaterialById(int id)
        {
            var textMaterial = await _unitOfWork.TextMaterialRepository.GetByIdWithDetailsAsync(id);

            if (textMaterial == null)
            {
                throw new CardFileException($"Text material with id {id} doesn't exist");
            }

            return _mapper.Map<TextMaterialDTO>(textMaterial);
        }

        public async Task<TextMaterialDTO> CreateTextMaterial(CreateTextMaterialDTO textMaterialDTO)
        {
            var textMaterial = _mapper.Map<TextMaterial>(textMaterialDTO);

            var category = await _unitOfWork.TextMaterialCategoryRepository.GetByTitleAsync(textMaterialDTO.CategoryTitle);
            var author = await _userManager.FindByIdAsync(textMaterialDTO.AuthorId);

            textMaterial.TextMaterialCategory = category;
            textMaterial.Author = author;

            try
            {
                await _unitOfWork.TextMaterialRepository.CreateAsync(textMaterial);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new CardFileException(ex.Message);
            }

            return _mapper.Map<TextMaterialDTO>(textMaterial);
        }
    }
}
