<template>
	<div class="file-manager-container">
		<splitpanes class="default-theme">
			<pane size="20">
				<CategoryTree ref="categoryTreeRef" @node-click="nodeClick" />
			</pane>
			<pane size="80" style="overflow: auto">
				<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
					<el-form :model="state.queryParams" ref="queryForm" :inline="true">
						<el-form-item label="文件名称">
							<el-input v-model="state.queryParams.fileName" placeholder="文件名称" clearable />
						</el-form-item>
						<el-form-item label="文件类型">
							<el-input v-model="state.queryParams.suffix" placeholder="文件类型" clearable />
						</el-form-item>
						<el-form-item label="存储位置">
							<el-input v-model="state.queryParams.filePath" placeholder="存储位置" clearable />
						</el-form-item>
						<el-form-item>
							<el-button-group>
								<el-button type="primary" icon="ele-Search" @click="handleQuery"> 查询 </el-button>
								<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
							</el-button-group>
						</el-form-item>
						<el-form-item>
							<el-upload
								:action="uploadUrl"
								:headers="uploadHeaders"
								:data="uploadData"
								:on-success="handleUploadSuccess"
								:show-file-list="false"
								multiple
							>
								<el-button type="primary" icon="ele-Upload" v-auth="'sysFile:add'"> 上传文件 </el-button>
							</el-upload>
						</el-form-item>
					</el-form>
				</el-card>

				<el-card class="full-table" shadow="hover" style="margin-top: 5px">
					<el-table :data="state.fileData" style="width: 100%" v-loading="state.loading" border>
						<el-table-column prop="fileName" label="文件名称" min-width="200" header-align="center" show-overflow-tooltip />
						<el-table-column prop="suffix" label="文件类型" width="100" align="center" show-overflow-tooltip />
						<el-table-column prop="sizeKb" label="文件大小" width="120" align="center" show-overflow-tooltip>
							<template #default="scope">
								{{ formatFileSize(scope.row.sizeKb) }}
							</template>
						</el-table-column>
						<el-table-column prop="filePath" label="存储位置" min-width="150" align="center" show-overflow-tooltip />
						<el-table-column prop="bucketName" label="存储桶" width="100" align="center" show-overflow-tooltip />
						<el-table-column prop="createTime" label="上传时间" width="160" align="center" show-overflow-tooltip>
							<template #default="scope">
								<!-- {{ formatTime(scope.row.createTime) }} -->
							</template>
						</el-table-column>
						<el-table-column label="操作" width="280" fixed="right" align="center" show-overflow-tooltip>
							<template #default="scope">
								<el-button icon="ele-View" text type="primary" @click="previewFile(scope.row)" v-auth="'sysFile:detail'"> 预览 </el-button>
								<el-button icon="ele-Download" text type="primary" @click="downloadFile(scope.row)" v-auth="'sysFile:detail'"> 下载 </el-button>
								<el-button icon="ele-Edit" text type="primary" @click="editFileCategory(scope.row)" v-auth="'sysFile:update'"> 分类 </el-button>
								<el-button icon="ele-Delete" text type="danger" @click="delFile(scope.row)" v-auth="'sysFile:delete'"> 删除 </el-button>
							</template>
						</el-table-column>
					</el-table>

					<!-- 分页 -->
					<el-pagination
						v-model:current-page="state.tableParams.page"
						v-model:page-size="state.tableParams.pageSize"
						:page-sizes="[10, 20, 50, 100]"
						:total="state.tableParams.total"
						layout="total, sizes, prev, pager, next, jumper"
						@size-change="handleSizeChange"
						@current-change="handleCurrentChange"
						style="margin-top: 15px"
					/>
				</el-card>
			</pane>
		</splitpanes>

		<!-- 文件分类编辑对话框 -->
		<el-dialog v-model="state.editCategoryDialogVisible" title="设置文件分类" width="500px">
			<el-form :model="state.editFileForm" label-width="100px">
				<el-form-item label="文件名称">
					<el-input v-model="state.editFileForm.fileName" disabled />
				</el-form-item>
				<el-form-item label="选择分类">
					<el-tree-select
						v-model="state.editFileForm.categoryId"
						:data="state.categoryTreeData"
						:props="{ value: 'id', label: 'name', children: 'children' }"
						placeholder="请选择分类"
						check-strictly
						clearable
					/>
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.editCategoryDialogVisible = false">取消</el-button>
				<el-button type="primary" @click="saveFileCategory">确定</el-button>
			</template>
		</el-dialog>

		<!-- 文件预览对话框 -->
		<el-dialog v-model="state.previewDialogVisible" :title="state.previewFileName" width="80%" top="5vh">
			<div class="preview-container">
				<!-- 图片预览 -->
				<el-image
					v-if="isImageFile(state.previewFile)"
					:src="getFileUrl(state.previewFile)"
					style="width: 100%; max-height: 600px"
					fit="contain"
				/>
				<!-- 其他文件类型提示 -->
				<div v-else class="preview-placeholder">
					<el-icon size="64"><Document /></el-icon>
					<p>此文件类型不支持预览</p>
					<el-button type="primary" @click="downloadFile(state.previewFile)">下载文件</el-button>
				</div>
			</div>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="fileManager">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { getAPI } from '/@/utils/axios-utils';
import { SysCategoryApi, SysFileApi } from '/@/api-services/api';
import { SysCategory, SysFile, PageFileInput, DeleteFileInput } from '/@/api-services/models';
import CategoryTree from '/@/views/category/component/categoryTree.vue';
import { Session } from '/@/utils/storage';
import { Document } from '@element-plus/icons-vue';

const categoryTreeRef = ref<InstanceType<typeof CategoryTree>>();
const state = reactive({
	loading: false,
	fileData: [] as Array<SysFile>, // 文件列表数据
	categoryTreeData: [] as Array<SysCategory>, // 分类树所有数据
	queryParams: {
		fileName: undefined,
		suffix: undefined,
		filePath: undefined,
		categoryId: undefined, // 选中的分类ID
	},
	tableParams: {
		page: 1,
		pageSize: 20,
		total: 0,
	},
	// 文件分类编辑
	editCategoryDialogVisible: false,
	editFileForm: {
		id: 0,
		fileName: '',
		categoryId: undefined,
	},
	// 文件预览
	previewDialogVisible: false,
	previewFileName: '',
	previewFile: {} as SysFile,
});

// 上传配置
const uploadUrl = ref('/api/sysFile/uploadFile');
const uploadHeaders = ref({
	Authorization: `Bearer ${Session.get('token')}`,
});
const uploadData = ref({
	categoryId: undefined, // 当前选中的分类ID
});

onMounted(async () => {
	await loadCategoryTree();
	handleQuery();
});

// 加载分类树数据
const loadCategoryTree = async () => {
	const res = await getAPI(SysCategoryApi).apiSysCategoryListGet(0);
	state.categoryTreeData = res.data.result ?? [];
};

// 查询操作
const handleQuery = async () => {
	state.loading = true;
	try {
		const params: PageFileInput = {
			page: state.tableParams.page,
			pageSize: state.tableParams.pageSize,
			fileName: state.queryParams.fileName,
			suffix: state.queryParams.suffix,
			filePath: state.queryParams.filePath,
		};
		
		// 如果选中了分类，添加分类筛选条件
		if (state.queryParams.categoryId) {
			// 这里可以根据实际API调整，可能需要通过relationId或其他字段关联分类
			params.keyword = `categoryId:${state.queryParams.categoryId}`;
		}
		
		const res = await getAPI(SysFileApi).apiSysFilePagePost(params);
		state.fileData = res.data.result?.items ?? [];
		state.tableParams.total = res.data.result?.total ?? 0;
	} catch (error) {
		console.error('查询文件列表失败:', error);
		ElMessage.error('查询文件列表失败');
	} finally {
		state.loading = false;
	}
};

// 重置操作
const resetQuery = () => {
	state.queryParams.fileName = undefined;
	state.queryParams.suffix = undefined;
	state.queryParams.filePath = undefined;
	state.queryParams.categoryId = undefined;
	state.tableParams.page = 1;
	uploadData.value.categoryId = undefined;
	handleQuery();
};

// 树组件点击
const nodeClick = async (node: any) => {
	state.queryParams.categoryId = node.id;
	uploadData.value.categoryId = node.id; // 设置上传时的分类ID
	state.tableParams.page = 1;
	handleQuery();
};

// 文件大小格式化
const formatFileSize = (sizeKb: number | undefined): string => {
	if (!sizeKb) return '0 KB';
	if (sizeKb < 1024) return `${sizeKb} KB`;
	if (sizeKb < 1024 * 1024) return `${(sizeKb / 1024).toFixed(2)} MB`;
	return `${(sizeKb / (1024 * 1024)).toFixed(2)} GB`;
};

// 获取文件URL
const getFileUrl = (file: SysFile): string => {
	if (file.bucketName === 'Local') {
		return `/${file.filePath}/${file.id}${file.suffix}`;
	} else {
		return file.url || '';
	}
};

// 判断是否为图片文件
const isImageFile = (file: SysFile): boolean => {
	const imageExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp'];
	return imageExtensions.includes(file.suffix?.toLowerCase() || '');
};

// 预览文件
const previewFile = (file: SysFile) => {
	state.previewFile = file;
	state.previewFileName = `${file.fileName}${file.suffix}`;
	state.previewDialogVisible = true;
};

// 下载文件
const downloadFile = async (file: SysFile) => {
	try {
		const response = await getAPI(SysFileApi).apiSysFileDownloadFilePost(file);
		// 创建下载链接
		const url = getFileUrl(file);
		const link = document.createElement('a');
		link.href = url;
		link.download = `${file.fileName}${file.suffix}`;
		link.click();
		ElMessage.success('下载开始');
	} catch (error) {
		console.error('下载文件失败:', error);
		ElMessage.error('下载文件失败');
	}
};

// 编辑文件分类
const editFileCategory = (file: SysFile) => {
	state.editFileForm.id = file.id || 0;
	state.editFileForm.fileName = `${file.fileName}${file.suffix}`;
	state.editFileForm.categoryId = file.relationId; // 假设relationId存储分类ID
	state.editCategoryDialogVisible = true;
};

// 保存文件分类
const saveFileCategory = async () => {
	try {
		// 更新文件的分类关联
		const updateData: SysFile = {
			id: state.editFileForm.id,
			relationId: state.editFileForm.categoryId,
			relationName: 'category', // 关联对象名称
		};
		
		await getAPI(SysFileApi).apiSysFileUpdatePost(updateData);
		ElMessage.success('分类设置成功');
		state.editCategoryDialogVisible = false;
		handleQuery();
	} catch (error) {
		console.error('设置文件分类失败:', error);
		ElMessage.error('设置文件分类失败');
	}
};

// 删除文件
const delFile = (file: SysFile) => {
	ElMessageBox.confirm(`确定删除文件：【${file.fileName}${file.suffix}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			const deleteData: DeleteFileInput = { id: file.id || 0 };
			await getAPI(SysFileApi).apiSysFileDeletePost(deleteData);
			ElMessage.success('删除成功');
			handleQuery();
		})
		.catch(() => {});
};

// 上传成功回调
const handleUploadSuccess = (response: any) => {
	ElMessage.success('文件上传成功');
	handleQuery(); // 刷新文件列表
};

// 改变页面容量
const handleSizeChange = (val: number) => {
	state.tableParams.pageSize = val;
	state.tableParams.page = 1;
	handleQuery();
};

// 改变页码序号
const handleCurrentChange = (val: number) => {
	state.tableParams.page = val;
	handleQuery();
};
</script>

<style scoped lang="scss">
.file-manager-container {
	height: calc(100vh - 84px);
	.full-table {
		height: calc(100vh - 180px);
	}
}

.preview-container {
	text-align: center;
	.preview-placeholder {
		padding: 50px;
		color: #999;
		p {
			margin: 20px 0;
			font-size: 16px;
		}
	}
}
</style>