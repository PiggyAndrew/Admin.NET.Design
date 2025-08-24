<template>
  <div class="files-container">
    <!-- 顶部导航栏 -->
    <div class="top-nav">
      <div class="nav-tabs">
        <div class="nav-item active">Revit</div>
        <div class="nav-item">族库设计</div>
        <div class="nav-item">DIMMAKI</div>
        <div class="nav-item">SketchUp</div>
      </div>
      <div class="nav-right">
        <span class="nav-link">你好朋友</span>
      </div>
    </div>

    <!-- 搜索区域 -->
    <div class="search-section">
      <div class="search-container">
        <el-input
          placeholder="找族件，就上数维族件站"
          class="search-input"
          size="large"
        >
          <template #append>
            <el-button :icon="Search" />
          </template>
        </el-input>
      </div>
    </div>

    <!-- 主要内容区域 -->
    <div class="main-content">
      <!-- 左侧分类树 -->
      <div class="sidebar">
        <div class="category-section">
          <div class="category-item" 
               v-for="category in categories" 
               :key="category.id"
               :class="{ active: selectedCategory === category.id }"
               @click="selectCategory(category.id)">
            <el-icon v-if="category.icon"><component :is="category.icon" /></el-icon>
            <span class="category-name">{{ category.name }}</span>
            <span class="category-count">({{ category.count }})</span>
          </div>
        </div>
      </div>

      <!-- 右侧内容区域 -->
      <div class="content-area">
        <!-- 筛选区域 -->
        <div class="filter-section">
          <div class="filter-tabs">
            <span class="filter-label">适用：</span>
            <el-radio-group  size="small">
              <el-radio-button label="全部软件">全部软件</el-radio-button>
              <el-radio-button label="Revit">Revit</el-radio-button>
            </el-radio-group>
          </div>
          
          <div class="filter-options">
            <el-checkbox-group >
              <el-checkbox label="国标族件">国标族件</el-checkbox>
              <el-checkbox label="品牌族件">品牌族件</el-checkbox>
              <el-checkbox label="可编辑族件">可编辑族件</el-checkbox>
              <el-checkbox label="参数化族件">参数化族件</el-checkbox>
            </el-checkbox-group>
          </div>
        </div>

        <!-- 品牌区域 -->
        <div class="brands-section">
          <div class="section-title">
            <span>品牌：</span>
            <div class="brand-tags">
              <el-tag 
                v-for="brand in brands" 
                :key="brand.id"
                :type="brand.type"
                size="small"
                class="brand-tag"
                @click="selectBrand(brand.id)"
              >
                <img v-if="brand.logo" :src="brand.logo" :alt="brand.name" class="brand-logo">
                {{ brand.name }}
              </el-tag>
            </div>
          </div>
        </div>

        <!-- 搜索结果信息 -->
        <div class="search-info">
          <div class="search-tags">
            <el-tag closable >搜索族件 Revit</el-tag>
            <el-tag closable >族文件</el-tag>
          </div>
          <div class="result-info">
            <span class="result-text">相关结果 <strong>27870</strong> 条</span>
            <div class="view-options">
              <span>排序</span>
              <span>综合排序</span>
            </div>
          </div>
        </div>

        <!-- 文件网格 -->
        <div class="files-grid">
          <div 
            v-for="file in fileList" 
            :key="file.id"
            class="file-card"
            @click="selectFile(file)"
          >
            <div class="file-preview">
              <img :src="file.thumbnail" :alt="file.name" class="file-thumbnail">
              <div class="file-actions">
                <el-button :icon="Download" size="small" circle></el-button>
                <el-button :icon="View" size="small" circle></el-button>
                <el-button :icon="Star" size="small" circle></el-button>
                <el-button :icon="Share" size="small" circle></el-button>
              </div>
            </div>
            <div class="file-info">
              <div class="file-title">{{ file.name }}</div>
              <div class="file-meta">
                <span class="file-author">{{ file.author }}</span>
                <span class="file-software">适用软件：{{ file.software }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- 分页 -->
        <div class="pagination-section">
          <el-pagination
            v-model:current-page="currentPage"
            v-model:page-size="pageSize"
            :page-sizes="[12, 24, 48, 96]"
            :total="totalFiles"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleSizeChange"
            @current-change="handleCurrentChange"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import {
  Search,
  Download,
  View,
  Star,
  Share,
  Folder,
  Document,
  Files,
  Picture,
  Grid
} from '@element-plus/icons-vue'

// 搜索关键词
const searchKeyword = ref('')

// 选中的分类
const selectedCategory = ref('all')

// 选中的软件
const selectedSoftware = ref('全部软件')

// 选中的筛选条件
const selectedFilters = ref([])

// 分页信息
const currentPage = ref(1)
const pageSize = ref(24)
const totalFiles = ref(27870)

// 分类数据
const categories = ref([
  { id: 'all', name: '全部内容', count: 27870, icon: 'Folder' },
  { id: 'family', name: '族库', count: 3602, icon: 'Document' },
  { id: 'template', name: '样板', count: 1228, icon: 'Files' },
  { id: 'decoration', name: '装饰', count: 2941, icon: 'Picture' },
  { id: 'electrical', name: '配电', count: 5213, icon: 'Grid' },
  { id: 'structure', name: '结构体', count: 4417, icon: 'Grid' },
  { id: 'hvac', name: '暖气', count: 1930, icon: 'Grid' },
  { id: 'plumbing', name: '给排', count: 640, icon: 'Grid' },
  { id: 'construction', name: '施工', count: 1290, icon: 'Grid' },
  { id: 'landscape', name: '景观', count: 378, icon: 'Grid' },
  { id: 'other', name: '其他', count: 7181, icon: 'Grid' }
])

// 品牌数据
const brands = ref([
  { id: 1, name: '上海朗绿', type: 'primary', logo: '' },
  { id: 2, name: '西门子', type: 'success', logo: '' },
  { id: 3, name: 'Midea', type: 'info', logo: '' },
  { id: 4, name: '德力西', type: 'warning', logo: '' },
  { id: 5, name: 'Tenda', type: 'danger', logo: '' },
  { id: 6, name: '更多', type: 'info', logo: '' }
])

// 文件列表数据
const fileList = ref([
  {
    id: 1,
    name: 'KQH水泵（立式）',
    author: 'BimCube',
    software: 'R',
    thumbnail: '/api/placeholder/200/150'
  },
  {
    id: 2,
    name: '钢板_钢板插 TS_MX_2404',
    author: 'BimCube',
    software: 'R',
    thumbnail: '/api/placeholder/200/150'
  },
  {
    id: 3,
    name: 'KQWH卧式化工泵',
    author: 'BimCube',
    software: 'R',
    thumbnail: '/api/placeholder/200/150'
  },
  {
    id: 4,
    name: '新风_主动火灾探测器_J5_APB_TS2001(Ex)',
    author: 'BimCube',
    software: 'R',
    thumbnail: '/api/placeholder/200/150'
  },
  {
    id: 5,
    name: '配电箱',
    author: 'BimCube',
    software: 'R',
    thumbnail: '/api/placeholder/200/150'
  }
])

// 方法
const handleSearch = () => {
  console.log('搜索:', searchKeyword.value)
}

const selectCategory = (categoryId: string) => {
  selectedCategory.value = categoryId
  console.log('选择分类:', categoryId)
}

const selectBrand = (brandId: number) => {
  console.log('选择品牌:', brandId)
}

const removeSearchTag = (tag: string) => {
  console.log('移除搜索标签:', tag)
}

const selectFile = (file: any) => {
  console.log('选择文件:', file)
}

const handleSizeChange = (size: number) => {
  pageSize.value = size
  console.log('页面大小变化:', size)
}

const handleCurrentChange = (page: number) => {
  currentPage.value = page
  console.log('页面变化:', page)
}

// 生成更多模拟数据
const generateMoreFiles = () => {
  const moreFiles = []
  for (let i = 6; i <= 24; i++) {
    moreFiles.push({
      id: i,
      name: `文件名称 ${i}`,
      author: 'BimCube',
      software: 'R',
      thumbnail: '/api/placeholder/200/150'
    })
  }
  fileList.value.push(...moreFiles)
}

onMounted(() => {
  generateMoreFiles()
})
</script>

<style scoped lang="scss">
.files-container {
  min-height: 100vh;
  background-color: #f5f7fa;
}

// 顶部导航
.top-nav {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  padding: 0 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 60px;
  color: white;
  
  .nav-tabs {
    display: flex;
    gap: 30px;
    
    .nav-item {
      padding: 8px 16px;
      cursor: pointer;
      border-radius: 4px;
      transition: background-color 0.3s;
      
      &.active {
        background-color: rgba(255, 255, 255, 0.2);
        font-weight: 600;
      }
      
      &:hover {
        background-color: rgba(255, 255, 255, 0.1);
      }
    }
  }
  
  .nav-right {
    .nav-link {
      cursor: pointer;
      &:hover {
        text-decoration: underline;
      }
    }
  }
}

// 搜索区域
.search-section {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  padding: 20px;
  
  .search-container {
    max-width: 600px;
    margin: 0 auto;
    
    .search-input {
      :deep(.el-input__wrapper) {
        border-radius: 25px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
      }
      
      :deep(.el-input-group__append) {
        border-radius: 0 25px 25px 0;
        background: #409eff;
        border-color: #409eff;
        
        .el-button {
          background: transparent;
          border: none;
          color: white;
        }
      }
    }
  }
}

// 主要内容
.main-content {
  display: flex;
  min-height: calc(100vh - 140px);
}

// 左侧边栏
.sidebar {
  width: 250px;
  background: white;
  border-right: 1px solid #e4e7ed;
  padding: 20px 0;
  
  .category-section {
    .category-item {
      display: flex;
      align-items: center;
      padding: 12px 20px;
      cursor: pointer;
      transition: all 0.3s;
      
      &:hover {
        background-color: #f5f7fa;
      }
      
      &.active {
        background-color: #ecf5ff;
        color: #409eff;
        border-right: 3px solid #409eff;
      }
      
      .el-icon {
        margin-right: 8px;
        font-size: 16px;
      }
      
      .category-name {
        flex: 1;
        font-size: 14px;
      }
      
      .category-count {
        font-size: 12px;
        color: #909399;
      }
    }
  }
}

// 内容区域
.content-area {
  flex: 1;
  padding: 20px;
  background: white;
}

// 筛选区域
.filter-section {
  margin-bottom: 20px;
  
  .filter-tabs {
    display: flex;
    align-items: center;
    margin-bottom: 15px;
    
    .filter-label {
      margin-right: 10px;
      font-weight: 500;
    }
  }
  
  .filter-options {
    :deep(.el-checkbox) {
      margin-right: 20px;
    }
  }
}

// 品牌区域
.brands-section {
  margin-bottom: 20px;
  
  .section-title {
    display: flex;
    align-items: center;
    
    > span {
      margin-right: 10px;
      font-weight: 500;
    }
    
    .brand-tags {
      display: flex;
      gap: 8px;
      flex-wrap: wrap;
      
      .brand-tag {
        cursor: pointer;
        display: flex;
        align-items: center;
        
        .brand-logo {
          width: 16px;
          height: 16px;
          margin-right: 4px;
        }
      }
    }
  }
}

// 搜索信息
.search-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  padding-bottom: 15px;
  border-bottom: 1px solid #e4e7ed;
  
  .search-tags {
    display: flex;
    gap: 8px;
  }
  
  .result-info {
    display: flex;
    align-items: center;
    gap: 20px;
    
    .result-text {
      font-size: 14px;
      color: #606266;
    }
    
    .view-options {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 14px;
      color: #909399;
    }
  }
}

// 文件网格
.files-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.file-card {
  border: 1px solid #e4e7ed;
  border-radius: 8px;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.3s;
  background: white;
  
  &:hover {
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
    transform: translateY(-2px);
    
    .file-actions {
      opacity: 1;
    }
  }
  
  .file-preview {
    position: relative;
    height: 150px;
    overflow: hidden;
    
    .file-thumbnail {
      width: 100%;
      height: 100%;
      object-fit: cover;
      background: #f5f7fa;
    }
    
    .file-actions {
      position: absolute;
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%);
      display: flex;
      gap: 8px;
      opacity: 0;
      transition: opacity 0.3s;
      
      .el-button {
        background: rgba(255, 255, 255, 0.9);
        border: none;
        
        &:hover {
          background: white;
        }
      }
    }
  }
  
  .file-info {
    padding: 12px;
    
    .file-title {
      font-size: 14px;
      font-weight: 500;
      color: #303133;
      margin-bottom: 8px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }
    
    .file-meta {
      display: flex;
      justify-content: space-between;
      align-items: center;
      font-size: 12px;
      color: #909399;
      
      .file-author {
        color: #606266;
      }
      
      .file-software {
        background: #409eff;
        color: white;
        padding: 2px 6px;
        border-radius: 4px;
        font-size: 10px;
      }
    }
  }
}

// 分页
.pagination-section {
  display: flex;
  justify-content: center;
  padding: 20px 0;
}

// 响应式设计
@media (max-width: 768px) {
  .main-content {
    flex-direction: column;
  }
  
  .sidebar {
    width: 100%;
    
    .category-section {
      display: flex;
      overflow-x: auto;
      padding: 0 20px;
      
      .category-item {
        white-space: nowrap;
        min-width: auto;
      }
    }
  }
  
  .files-grid {
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
    gap: 15px;
  }
}
</style>