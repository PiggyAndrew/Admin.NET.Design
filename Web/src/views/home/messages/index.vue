<template>
  <div class="dashboard-container">
    <!-- 数据分析统计卡片 -->
    <div class="stats-section">
      <div class="section-header">
        <h3 class="section-title">
          <el-icon class="title-icon"><TrendCharts /></el-icon>
          数据分析
        </h3>
        <div class="section-subtitle">实时数据统计概览</div>
      </div>
      
      <el-row :gutter="20" class="stats-cards">
        <el-col :span="6">
          <el-card class="stat-card revit-family" @click="handleCardClick('revit-family')">
            <div class="stat-content">
              <div class="stat-icon">
                <el-icon><Document /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-label">Revit族数量</div>
                <div class="stat-value" ref="value1">999</div>
                <div class="stat-subtitle">当前族文件总数</div>
              </div>
              <div class="stat-trend up">
                <el-icon><ArrowUp /></el-icon>
                <span>+12%</span>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :span="6">
          <el-card class="stat-card revit-template" @click="handleCardClick('revit-template')">
            <div class="stat-content">
              <div class="stat-icon">
                <el-icon><Files /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-label">Revit样板数量</div>
                <div class="stat-value" ref="value2">99</div>
                <div class="stat-subtitle">当前样板文件总数</div>
              </div>
              <div class="stat-trend up">
                <el-icon><ArrowUp /></el-icon>
                <span>+8%</span>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :span="6">
          <el-card class="stat-card revit-station" @click="handleCardClick('revit-station')">
            <div class="stat-content">
              <div class="stat-icon">
                <el-icon><Picture /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-label">Revit站图数量</div>
                <div class="stat-value" ref="value3">999</div>
                <div class="stat-subtitle">当前站图文件总数</div>
              </div>
              <div class="stat-trend up">
                <el-icon><ArrowUp /></el-icon>
                <span>+15%</span>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :span="6">
          <el-card class="stat-card cad-blocks" @click="handleCardClick('cad-blocks')">
            <div class="stat-content">
              <div class="stat-icon">
                <el-icon><Grid /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-label">CAD图块数量</div>
                <div class="stat-value" ref="value4">999</div>
                <div class="stat-subtitle">当前图块文件总数</div>
              </div>
              <div class="stat-trend up">
                <el-icon><ArrowUp /></el-icon>
                <span>+20%</span>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>
      
      <el-row :gutter="20" class="stats-cards">
        <el-col :span="6">
          <el-card class="stat-card enterprise" @click="handleCardClick('enterprise')">
            <div class="stat-content">
              <div class="stat-icon">
                <el-icon><OfficeBuilding /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-label">已登记企业数</div>
                <div class="stat-value" ref="value5">9</div>
                <div class="stat-subtitle">用户活跃度</div>
              </div>
              <div class="stat-trend up">
                <el-icon><ArrowUp /></el-icon>
                <span>+5%</span>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :span="6">
          <el-card class="stat-card users" @click="handleCardClick('users')">
            <div class="stat-content">
              <div class="stat-icon">
                <el-icon><User /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-label">已登记用户数</div>
                <div class="stat-value" ref="value6">9</div>
                <div class="stat-subtitle">用户活跃度</div>
              </div>
              <div class="stat-trend up">
                <el-icon><ArrowUp /></el-icon>
                <span>+3%</span>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :span="12">
          <el-card class="quick-actions-card">
            <div class="quick-actions">
              <div class="actions-title">
                <el-icon><Lightning /></el-icon>
                快捷操作
              </div>
              <div class="actions-list">
                <el-button type="primary" :icon="Upload" size="small">上传文件</el-button>
                <el-button type="success" :icon="Download" size="small">导出数据</el-button>
                <el-button type="warning" :icon="Setting" size="small">系统设置</el-button>
                <el-button type="info" :icon="View" size="small">查看报告</el-button>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>
    </div>

    <el-row :gutter="20" class="main-content">
      <!-- 用户活跃度图表 -->
      <el-col :span="14">
        <el-card class="chart-card">
          <template #header>
            <div class="card-header">
              <div class="header-left">
                <el-icon class="header-icon"><TrendCharts /></el-icon>
                <span>用户活跃度</span>
              </div>
              <div class="header-right">
                <el-tag type="success" size="small">实时更新</el-tag>
                <el-dropdown>
                  <el-button type="text" :icon="MoreFilled" size="small"></el-button>
                  <template #dropdown>
                    <el-dropdown-menu>
                      <el-dropdown-item>导出图表</el-dropdown-item>
                      <el-dropdown-item>全屏查看</el-dropdown-item>
                      <el-dropdown-item>设置</el-dropdown-item>
                    </el-dropdown-menu>
                  </template>
                </el-dropdown>
              </div>
            </div>
          </template>
          <div class="chart-container">
            <div ref="chartRef" style="width: 100%; height: 300px;"></div>
          </div>
        </el-card>
        
        <!-- 问题反馈提示 -->
        <el-card class="feedback-card" style="margin-top: 20px;">
          <div class="feedback-header">
            <el-icon class="feedback-icon"><Warning /></el-icon>
            <span class="feedback-title">问题反馈提示</span>
            <el-tag type="warning" size="small">需要关注</el-tag>
          </div>
          <div class="feedback-content">
            <div class="feedback-item">
              <div class="feedback-meta">
                <el-tag type="danger" size="small">紧急</el-tag>
                <span class="feedback-date">2025-07-16</span>
              </div>
              <div class="feedback-text">
                <strong>问题：白屏问题</strong>
                <p>这是第一个问题，问题是关于系统白屏的情况，需要及时处理</p>
              </div>
            </div>
            <div class="feedback-item">
              <div class="feedback-meta">
                <el-tag type="warning" size="small">重要</el-tag>
                <span class="feedback-date">2025-07-16</span>
              </div>
              <div class="feedback-text">
                <p>这是第二个问题，关于数据加载缓慢的问题</p>
              </div>
            </div>
            <div class="feedback-item">
              <div class="feedback-meta">
                <el-tag type="info" size="small">一般</el-tag>
                <span class="feedback-date">2025-07-16</span>
              </div>
              <div class="feedback-text">
                <p>这是第三个问题，关于界面优化建议</p>
              </div>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <!-- 操作日志列表 -->
      <el-col :span="10">
        <el-card class="log-card">
          <template #header>
            <div class="card-header">
              <div class="header-left">
                <el-icon class="header-icon"><List /></el-icon>
                <span>问题反馈列表</span>
              </div>
              <div class="header-right">
                <el-button type="primary" size="small" :icon="Plus">查看更多问题</el-button>
              </div>
            </div>
          </template>
          <div class="log-list">
            <div class="log-item" v-for="(item, index) in logList" :key="index">
              <div class="log-avatar">
                <el-avatar :size="32" :src="item.avatar">
                  <el-icon><User /></el-icon>
                </el-avatar>
              </div>
              <div class="log-content">
                <div class="log-header">
                  <span class="log-title">{{ item.content }}</span>
                  <el-tag :type="item.type" size="small">{{ item.status }}</el-tag>
                </div>
                <div class="log-meta">
                  <span class="log-time">
                    <el-icon><Clock /></el-icon>
                    {{ item.time }}
                  </span>
                  <span class="log-user">{{ item.user }}</span>
                </div>
              </div>
              <div class="log-action">
                <el-button type="primary" link size="small" :icon="View">详情</el-button>
              </div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue'
import * as echarts from 'echarts'
import {
  Document,
  Files,
  Picture,
  Grid,
  OfficeBuilding,
  User,
  TrendCharts,
  ArrowUp,
  Lightning,
  Upload,
  Download,
  Setting,
  View,
  Warning,
  List,
  Plus,
  Clock,
  MoreFilled
} from '@element-plus/icons-vue'

const chartRef = ref<HTMLElement>()

// 模拟日志数据
const logList = ref([
  { 
    time: '2025-07-16 14:00', 
    content: '数据库备份异常', 
    user: '系统管理员',
    status: '已处理',
    type: 'success',
    avatar: ''
  },
  { 
    time: '2025-07-16 13:45', 
    content: '用户登录失败', 
    user: '张三',
    status: '处理中',
    type: 'warning',
    avatar: ''
  },
  { 
    time: '2025-07-16 13:30', 
    content: '文件上传错误', 
    user: '李四',
    status: '待处理',
    type: 'danger',
    avatar: ''
  },
  { 
    time: '2025-07-16 13:15', 
    content: '系统性能监控', 
    user: '系统',
    status: '正常',
    type: 'info',
    avatar: ''
  },
  { 
    time: '2025-07-16 13:00', 
    content: '定时任务执行', 
    user: '系统',
    status: '已完成',
    type: 'success',
    avatar: ''
  },
  { 
    time: '2025-07-16 12:45', 
    content: '数据同步任务', 
    user: '系统',
    status: '已完成',
    type: 'success',
    avatar: ''
  },
  { 
    time: '2025-07-16 12:30', 
    content: '用户权限变更', 
    user: '王五',
    status: '已审核',
    type: 'info',
    avatar: ''
  },
  { 
    time: '2025-07-16 12:15', 
    content: '系统更新通知', 
    user: '系统',
    status: '已发布',
    type: 'success',
    avatar: ''
  }
])

// 卡片点击事件
const handleCardClick = (type: string) => {
  console.log('点击了卡片:', type)
}

// 初始化图表
const initChart = () => {
  if (!chartRef.value) return
  
  const chart = echarts.init(chartRef.value)
  
  const option = {
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: ['2025-07-10', '2025-07-12', '2025-07-14', '2025-07-16'],
      axisLine: {
        lineStyle: {
          color: '#e4e7ed'
        }
      },
      axisLabel: {
        color: '#606266'
      }
    },
    yAxis: {
      type: 'value',
      max: 60,
      axisLine: {
        lineStyle: {
          color: '#e4e7ed'
        }
      },
      axisLabel: {
        color: '#606266'
      },
      splitLine: {
        lineStyle: {
          color: '#f5f7fa'
        }
      }
    },
    series: [
      {
        data: [20, 50, 30, 40],
        type: 'line',
        smooth: true,
        lineStyle: {
          color: '#409eff',
          width: 3
        },
        itemStyle: {
          color: '#409eff',
          borderWidth: 2,
          borderColor: '#fff'
        },
        areaStyle: {
          color: {
            type: 'linear',
            x: 0,
            y: 0,
            x2: 0,
            y2: 1,
            colorStops: [
              {
                offset: 0,
                color: 'rgba(64, 158, 255, 0.4)'
              },
              {
                offset: 1,
                color: 'rgba(64, 158, 255, 0.05)'
              }
            ]
          }
        },
        markPoint: {
          data: [
            { type: 'max', name: '最大值' },
            { type: 'min', name: '最小值' }
          ]
        }
      }
    ]
  }
  
  chart.setOption(option)
  
  // 响应式处理
  window.addEventListener('resize', () => {
    chart.resize()
  })
}

onMounted(() => {
  nextTick(() => {
    initChart()
  })
})
</script>

<style scoped lang="scss">
.dashboard-container {
  padding: 24px;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  min-height: 100vh;
}

.section-header {
  margin-bottom: 24px;
  
  .section-title {
    display: flex;
    align-items: center;
    margin: 0 0 8px 0;
    font-size: 24px;
    font-weight: 700;
    color: #303133;
    
    .title-icon {
      margin-right: 12px;
      font-size: 28px;
      color: #409eff;
    }
  }
  
  .section-subtitle {
    color: #909399;
    font-size: 14px;
    margin-left: 40px;
  }
}

.stats-section {
  margin-bottom: 24px;
}

.stats-cards {
  margin-bottom: 20px;
}

.stat-card {
  border: none;
  border-radius: 16px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  transition: all 0.3s ease;
  cursor: pointer;
  overflow: hidden;
  position: relative;
  
  &::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 4px;
    background: linear-gradient(90deg, #409eff, #67c23a);
  }
  
  &:hover {
    transform: translateY(-4px);
    box-shadow: 0 8px 30px rgba(0, 0, 0, 0.12);
  }
  
  &.revit-family::before {
    background: linear-gradient(90deg, #409eff, #5dade2);
  }
  
  &.revit-template::before {
    background: linear-gradient(90deg, #67c23a, #58d68d);
  }
  
  &.revit-station::before {
    background: linear-gradient(90deg, #e6a23c, #f7dc6f);
  }
  
  &.cad-blocks::before {
    background: linear-gradient(90deg, #f56c6c, #ec7063);
  }
  
  &.enterprise::before {
    background: linear-gradient(90deg, #9b59b6, #bb8fce);
  }
  
  &.users::before {
    background: linear-gradient(90deg, #1abc9c, #76d7c4);
  }
  
  :deep(.el-card__body) {
    padding: 24px;
  }
}

.stat-content {
  display: flex;
  align-items: center;
  position: relative;
  
  .stat-icon {
    width: 60px;
    height: 60px;
    border-radius: 12px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: linear-gradient(135deg, #409eff, #67c23a);
    margin-right: 16px;
    
    .el-icon {
      font-size: 24px;
      color: white;
    }
  }
  
  .stat-info {
    flex: 1;
    
    .stat-label {
      font-size: 14px;
      color: #909399;
      margin-bottom: 8px;
      font-weight: 500;
    }
    
    .stat-value {
      font-size: 32px;
      font-weight: 700;
      color: #303133;
      margin-bottom: 4px;
      line-height: 1;
    }
    
    .stat-subtitle {
      font-size: 12px;
      color: #c0c4cc;
    }
  }
  
  .stat-trend {
    display: flex;
    align-items: center;
    padding: 4px 8px;
    border-radius: 8px;
    font-size: 12px;
    font-weight: 600;
    
    &.up {
      background: rgba(103, 194, 58, 0.1);
      color: #67c23a;
    }
    
    &.down {
      background: rgba(245, 108, 108, 0.1);
      color: #f56c6c;
    }
    
    .el-icon {
      margin-right: 4px;
    }
  }
}

.quick-actions-card {
  border: none;
  border-radius: 16px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  
  :deep(.el-card__body) {
    padding: 24px;
  }
}

.quick-actions {
  .actions-title {
    display: flex;
    align-items: center;
    color: white;
    font-size: 16px;
    font-weight: 600;
    margin-bottom: 16px;
    
    .el-icon {
      margin-right: 8px;
      font-size: 18px;
    }
  }
  
  .actions-list {
    display: flex;
    gap: 12px;
    flex-wrap: wrap;
    
    .el-button {
      border-radius: 8px;
      font-weight: 500;
    }
  }
}

.main-content {
  .chart-card,
  .log-card,
  .feedback-card {
    border: none;
    border-radius: 16px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  }
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  
  .header-left {
    display: flex;
    align-items: center;
    font-weight: 600;
    color: #303133;
    
    .header-icon {
      margin-right: 8px;
      font-size: 18px;
      color: #409eff;
    }
  }
  
  .header-right {
    display: flex;
    align-items: center;
    gap: 8px;
  }
}

.chart-container {
  padding: 16px 0;
}

.feedback-card {
  .feedback-header {
    display: flex;
    align-items: center;
    margin-bottom: 16px;
    padding-bottom: 12px;
    border-bottom: 1px solid #f0f2f5;
    
    .feedback-icon {
      margin-right: 8px;
      font-size: 18px;
      color: #e6a23c;
    }
    
    .feedback-title {
      flex: 1;
      font-weight: 600;
      color: #303133;
    }
  }
  
  .feedback-content {
    .feedback-item {
      padding: 12px 0;
      border-bottom: 1px solid #f5f7fa;
      
      &:last-child {
        border-bottom: none;
      }
      
      .feedback-meta {
        display: flex;
        align-items: center;
        gap: 8px;
        margin-bottom: 8px;
        
        .feedback-date {
          font-size: 12px;
          color: #909399;
        }
      }
      
      .feedback-text {
        p {
          margin: 4px 0;
          line-height: 1.6;
          color: #606266;
          
          &:first-child {
            margin-top: 0;
          }
          
          &:last-child {
            margin-bottom: 0;
          }
        }
        
        strong {
          color: #303133;
        }
      }
    }
  }
}

.log-list {
  max-height: 500px;
  overflow-y: auto;
  
  &::-webkit-scrollbar {
    width: 6px;
  }
  
  &::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 3px;
  }
  
  &::-webkit-scrollbar-thumb {
    background: #c1c1c1;
    border-radius: 3px;
    
    &:hover {
      background: #a8a8a8;
    }
  }
}

.log-item {
  display: flex;
  align-items: flex-start;
  padding: 16px 0;
  border-bottom: 1px solid #f5f7fa;
  transition: background-color 0.2s ease;
  
  &:hover {
    background-color: #fafbfc;
  }
  
  &:last-child {
    border-bottom: none;
  }
  
  .log-avatar {
    margin-right: 12px;
  }
  
  .log-content {
    flex: 1;
    
    .log-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 6px;
      
      .log-title {
        font-size: 14px;
        color: #303133;
        font-weight: 500;
      }
    }
    
    .log-meta {
      display: flex;
      align-items: center;
      gap: 12px;
      font-size: 12px;
      color: #909399;
      
      .log-time {
        display: flex;
        align-items: center;
        
        .el-icon {
          margin-right: 4px;
        }
      }
      
      .log-user {
        font-weight: 500;
      }
    }
  }
  
  .log-action {
    margin-left: 12px;
  }
}

// 动画效果
@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.stat-card {
  animation: fadeInUp 0.6s ease forwards;
  
  &:nth-child(1) { animation-delay: 0.1s; }
  &:nth-child(2) { animation-delay: 0.2s; }
  &:nth-child(3) { animation-delay: 0.3s; }
  &:nth-child(4) { animation-delay: 0.4s; }
}

.chart-card,
.log-card,
.feedback-card {
  animation: fadeInUp 0.6s ease forwards;
}
</style>